namespace compete_platform.Infrastructure.Services.LobbyErrorHandler
{
    public interface ILobbyErrorHandler
    {
        public Task HandleVetoFailed(long lobbyId, Exception ex);
    }
}
