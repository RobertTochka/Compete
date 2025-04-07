using AutoMapper;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Dto;
using System.Transactions;

namespace CompeteGameServerHandler.Infrastructure.Services
{
    public class ServerService : IServerService
    {
        private readonly CServerRepository _serverRep;
        private readonly IMapper _mapper;
        private readonly AppConfig _cfg;
        private readonly IServerRunner _runner;
        private readonly CLobbyRepository _lobbyRep;
        private readonly CUserRepository _userSrc;
        private static readonly long _convertConstantForSteam = 76561197960265728;
        private static readonly int _defaultPort = 27015;

        public ServerService(CServerRepository serverRep, 
            IMapper mapper,
            IServerRunner runner,
            AppConfig cfg,
            CLobbyRepository lobbyRep,
            CUserRepository userSrc
            ) 
        {
            _serverRep = serverRep;
            _mapper = mapper;
            _cfg = cfg;
            _runner = runner;
            _lobbyRep = lobbyRep;
            _userSrc = userSrc;
        }
        private static Map GameTitleMapToMap(string mapString)
        {
            var lowerCaseMapString = mapString.ToLower();

            if (lowerCaseMapString.Contains("mirage"))
                return Map.Mirage;
            if (lowerCaseMapString.Contains("inferno"))
                return Map.Inferno;
            if (lowerCaseMapString.Contains("nuke"))
                return Map.Nuke;
            if (lowerCaseMapString.Contains("anubis"))
                return Map.Anubis;
            if (lowerCaseMapString.Contains("overpass"))
                return Map.Overpass;
            if (lowerCaseMapString.Contains("vertigo"))
                return Map.Vertigo;
            if (lowerCaseMapString.Contains("ancient"))
                return Map.Ancient;
            if (lowerCaseMapString.Contains("dust2"))
                return Map.Dust2;
            if (lowerCaseMapString.Contains("office"))
                return Map.Office;
            if (lowerCaseMapString.Contains("italy"))
                return Map.Italy;

            throw new ArgumentException($"Unknown map: {mapString}");
        }
        private static int ConvertSteamIdToAccountId(long steamId) => (int)(steamId - _convertConstantForSteam);
        public async Task UpdateMatchInformation(GameLogInformation info, long lobbyId)
        {
            if (info.Map is null || info.ScoreCt is null || info.ScoreT is null)
                throw new ApplicationException(AppDictionary.GameStateNotFull);

            var lobby = await _lobbyRep.GetLobbyForLogInformation(lobbyId, GameTitleMapToMap(info.Map));
            lobby.Version = Guid.NewGuid();
            lobby.LastServerUpdate = DateTime.UtcNow;
            var maxScore = Math.Max((int)info.ScoreCt, (int)info.ScoreT);
            var minScore = Math.Min((int)info.ScoreT, (int)info.ScoreCt);
            var currentMatch = lobby.Matches.First();
            currentMatch.FirstTeamScore = (short)maxScore;
            currentMatch.SecondTeamScore = (short)minScore;
            var partipiciantAccountWinnerId = info.Players
                .First(c => c.Team
                    .Equals(info.TeamWinner)).AccountId;

            var teamWinner = currentMatch.Stats
                .Select(s => s.User)
                .First(u => ConvertSteamIdToAccountId(long.Parse(u!.SteamId))
                    .Equals(partipiciantAccountWinnerId))!.Teams.First().Id;

            foreach(var playerInfo in info.Players)
            {
                var neededStat = currentMatch.Stats.FirstOrDefault(s => playerInfo.AccountId
                .Equals(ConvertSteamIdToAccountId(long.Parse(s.User!.SteamId))));
                if (neededStat is null)
                    continue;
                _mapper.Map(playerInfo, neededStat);
            }
            currentMatch.TeamId = teamWinner;
            await _lobbyRep.SaveChangesAsync();
        }

        public async Task<InitialConfiguration> GetInitialConfigForGameServer(string ip, int port)
        {
            var config =  await _serverRep.GetInitialConfiguration(ip, port);
            config.AdminsId = await _userSrc.GetPlatformAdminsSteamIds();
            return config;
        }
        private int GetAvailablePort(List<LobbyPort> ports)
        {
            var startPort = _defaultPort;
            var availableForUse = Enumerable.Range(0, _cfg.MaxAmountAvailablePorts)
                .Select(t => t * 100 + startPort);
            var availableNow = availableForUse.Except(ports.Select(t => t.Port));
            if (!availableNow.Any())
                throw new ApplicationException(AppDictionary.GameServerUnderHighDemand);
            return availableNow.First();
        }
        public async Task StartServerAsync(int id, long lobbyId)
        {
            using var transaction = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
            var neededServer = await _serverRep.GetServerById(id) ??
                throw new ApplicationException(AppDictionary.GameServerNotFound);
            if (!neededServer.IsHealthy)
                throw new ApplicationException(AppDictionary.ServerNotHealthy);
            var port = GetAvailablePort(neededServer.PlayingPorts);
            neededServer.PlayingPorts.Add(new() { LobbyId = lobbyId,Port = port });
            await _serverRep.SaveChangesAsync();
            await _runner.RunServer(neededServer.Path, port, lobbyId);
            transaction.Complete();
        }

        public async Task StopServerAsync(int id, long lobbyId)
        {
            using var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
            var server = (await _serverRep.GetServerById(id))
                ?? throw new ApplicationException(AppDictionary.GameServerNotFound);
            var path = server.Path;
            var playingPort = server.PlayingPorts.First(pp => pp.LobbyId.Equals(lobbyId));
            var port = playingPort.Port;
            server.PlayingPorts.Remove(playingPort);
            await _serverRep.SaveChangesAsync();
            await _runner.StopServer(path, port, lobbyId);
            transaction.Complete();
        }
    }
}
