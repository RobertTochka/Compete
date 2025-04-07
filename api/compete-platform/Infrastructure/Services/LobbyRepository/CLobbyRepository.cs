using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_poco.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using SteamWebAPI2.Models.GameServers;

namespace compete_poco.Infrastructure.Services
{
    public abstract class CLobbyRepository : CRepository
    {
        public CLobbyRepository(ApplicationContext ctx) : base(ctx) { }
        public abstract Task<Lobby> GetLobbyByUserIdAsync(long userId);
        public abstract Task<GetUserDto[]> GetUsersInLobbyAsync(long lobbyId);
        public abstract Task<Lobby> GetLobbyByIdAsync(long id);
        public abstract Task<List<GetLobbyViewDto>> GetLobbiesAsync(GetLobbyRequest request);
        public abstract Task<List<GetLobbyViewDto>> GetAllLobbiesAsync();
        public abstract Task<List<long>> GetUsersIdInLobbyAsync(long lobbyId);
        public abstract Task<bool> UserAlreadyInLobby(long userId);
        public abstract Task<bool> UserIsCreatorOfLobby(long userId, long lobbyId);
        public abstract Task CreateLobbyAsync(Lobby newLobby);
        public abstract Task<Lobby?> GetLobbyForAggregation(long lobbyId, params LobbyStatus[] statuses);
        public abstract Task<Lobby?> GetLobbyForUpdating(long lobbyId);
        public abstract Task<UserBid?> GetUserBid(long userBid);
        public abstract Task<Team?> GetTeamForUpdating(long creatorId);
        public abstract Task<Lobby> GetLobbyForLogInformation(long lobbyId, Map map);
        public abstract Task<int> GetLobbyCountForPlayerAmount(Models.Type playersAmount);
        public abstract Task<long> GetStaledLobby(TimeSpan staleValue);
        public abstract Task<GetMatchView[]> GetMatchesForAdmin(GetMatchesForAdminRequest req);
        public abstract Task<int> GetLobbyCounts(string status);
        public abstract Task<decimal> GetLobbyComissions(string interval);
    }
}
