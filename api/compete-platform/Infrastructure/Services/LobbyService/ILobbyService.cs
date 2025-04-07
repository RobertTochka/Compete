using compete_platform.Dto;
using compete_poco.Dto;
using compete_poco.Infrastructure.Services.LobbyService.Models;

namespace compete_platform.Infrastructure.Services.LobbyService
{
    public interface ILobbyService
    {

        public Task<ActionInfo> JoinToLobby(JoinToLobbyInfo info);
        public Task<ActionInfo?> LeaveFromLobby(long userId, long lobbyId);
        public Task<ActionInfo> SetNewLobbyConfiguration(LobbyAdminConfiguration newCfg, long userId);
        public Task<ActionInfo> ChangeUserBid(ChangeUserBidRequest request);
        public Task<ActionInfo> ChangeTeamName(ChangeTeamNameRequest req);
        public Task<JoinToLobbyInfo> CreateInviteForUser(SendInviteRequest req);
        public Task<ActionInfo> StartMapPick(long lobbyId, long userId);
        public Task<ActionInfo> DoAction(MapPickRequest req);
        public Task<ActionInfo> GetLobbyVetoStateAsyncById(long lobbyId, long userId);
        public Task<ActionInfo> GetLobbyVetoStateAsyncByUserId(long userId);
        public abstract Task<List<GetLobbyViewDto>> GetLobbiesAsync(GetLobbyRequest request);
        public abstract Task<List<GetLobbyViewDto>> GetAllLobbiesAsync();
        public abstract Task<GetLobbyDto> CreateLobbyAsync(long creatorId);
        public abstract Task<ActionInfo> Cancel_Lobby(long lobbyId, long[]? offenders = null);
        public abstract Task<MapEndInformation> LobbyMapEnded(long lobbyId);
        public abstract Task AllConnectedConfirmation(long lobbyId);
        public Task<ActionInfo> ResetLobbyAfterFailedVeto(long lobbyId);
    }
}
