using CompeteGameServerHandler.Dto;

namespace CompeteGameServerHandler.Infrastructure.Services
{
    public interface IServerService
    {
        public Task UpdateMatchInformation(GameLogInformation info, long lobbyId);
        public Task<InitialConfiguration> GetInitialConfigForGameServer(string ip, int port);
        public Task StartServerAsync(int id, long lobbyId);
        public Task StopServerAsync(int id, long lobbyId);
    }
}
