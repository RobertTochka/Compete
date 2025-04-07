using compete_platform.Dto;

namespace CompeteGameServerHandler.Infrastructure.Services
{
    public interface IServerRunner
    {
        public Task<ServerRunnerResponse?> RunServer(string path, int port, long lobbyId);
        public Task<ServerRunnerResponse?> StopServer(string path, int port, long lobbyId);
        public Task<ServerRunnerResponse?> GetServerOutput(string path, int port, long lobbyId);
        public Task<bool> CheckServerHealthy(string path);
        public Task<GetDemoFileResponse> GetDemoFile(GetDemoFileRequest req);
    }
}
