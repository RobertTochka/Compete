using AutoMapper;
using compete_poco.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Dto;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Configuration;

namespace compete_poco.Infrastructure.Services
{
    public class ServerRepository : CServerRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _factory;
        private readonly HttpClient _client;
        private readonly AppConfig _cfg;
        private readonly IMapper _mapper;

        public ServerRepository(ApplicationContext ctx, 
            IDbContextFactory<ApplicationContext> factory,
            HttpClient client,
            AppConfig cfg,
            IMapper mapper) : base(ctx)
        {
            _factory = factory;
            _client = client;
            _cfg = cfg;
            _mapper = mapper;
        }

        public override async Task<InitialConfiguration> GetInitialConfiguration(string ip, int port)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var servers = await ctx.Servers.ToListAsync();
            var server = servers.FirstOrDefault(s => s.Path.Equals(ip) && s.PlayingPorts.Select(s => s.Port).Contains(port)) ??
                throw new ApplicationException(AppDictionary.GameServerNotFound);
            var neededLobbyId = server.PlayingPorts
                       .First(p => p.Port.Equals(port)).LobbyId;
            var cfgFromDb = await ctx.Lobbies
                .Where(l => l.Id.Equals(neededLobbyId))
                .Include(l => l.Teams)
                .ThenInclude(t => t.Users)
                .FirstAsync();
            cfgFromDb.MapActions = cfgFromDb.MapActions.Where(a => a.IsPicked).ToList();
            var cfg = _mapper.Map<InitialConfiguration>(cfgFromDb);
            return cfg;
        }

        public override async Task<Server?> GetServerById(int id)
        {
            var server = await _ctx.Servers.FirstOrDefaultAsync(s => s.Id.Equals(id));
            return server;
        }
        public override async Task<List<GetServerDto>> GetAvailableServersAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var servers = (await ctx.Servers
                .Where(s => s.IsHealthy)
                .ToListAsync())
                .Where(c => c.PlayingPorts.Count <= _cfg.MaxAmountAvailablePorts);
            var mapped = _mapper.Map<List<GetServerDto>>(servers);
            return mapped;
        }

        public async override Task<Server> CreateServerAsync(Server server)
        {
            await _ctx.Servers.AddAsync(server);
            return server;
        }

        public async override Task<GetServerPingDto> GetServerPingAsync(string ip)
        {
            using var ping = new Ping();
            try
            {
                PingReply reply = await ping.SendPingAsync(ip);
                return new GetServerPingDto
                {
                    Ip = ip,
                    PingTime = reply.Status == IPStatus.Success ? reply.RoundtripTime : -1,
                    Status = reply.Status.ToString(),
                    ErrorMessage = reply.Status == IPStatus.Success ? "No error" : "Ping failed"
                };
            }
            catch (Exception ex)
            {
                return new GetServerPingDto
                {
                    Ip = ip,
                    PingTime = -1,
                    Status = "Error",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async override Task<List<GetServerDto>> GetAllServers()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var servers = (await ctx.Servers
                .ToListAsync());
            var mapped = _mapper.Map<List<GetServerDto>>(servers);
            return mapped;
        }

        public async override Task UpdateServerHealthyStatus(string path, bool healthyStatus)
        {
            await _ctx.Database.ExecuteSqlRawAsync($@"
                UPDATE public.""Servers"" 
                SET ""IsHealthy"" = {healthyStatus} 
                WHERE ""Path"" = '{path}'
            ");
        }

        public async override Task<int> GetCountsOfTypedServers(bool? isHealthy)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var query = ctx.Servers.AsQueryable();
            if(isHealthy != null)
                query = query.Where(s => s.IsHealthy == isHealthy);
            return await query.CountAsync();
        }

        public async override Task<string?> GetServerPathByLobbyId(long lobbyId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var path = await ctx.Lobbies
                .Where(s => s.Id == lobbyId)
                .Select(l => l.Server!.Path)
                .FirstOrDefaultAsync();
            return path;
        }
    }
}
