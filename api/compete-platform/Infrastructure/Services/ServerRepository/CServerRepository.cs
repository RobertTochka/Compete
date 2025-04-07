using compete_poco.Dto;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Dto;

namespace compete_poco.Infrastructure.Services
{
    public abstract class CServerRepository : CRepository
    {
      
        public CServerRepository(ApplicationContext ctx) : base(ctx)
        {
            
        }
        public abstract Task<List<GetServerDto>> GetAvailableServersAsync();
        public abstract Task<InitialConfiguration> GetInitialConfiguration(string ip, int port);
        public abstract Task<Server?> GetServerById(int id);
        public abstract Task<Server> CreateServerAsync(Server server);
        public abstract Task<GetServerPingDto> GetServerPingAsync(string ip);
        public abstract Task<List<GetServerDto>> GetAllServers();
        public abstract Task UpdateServerHealthyStatus(string path, bool healthyStatus);
        public abstract Task<int> GetCountsOfTypedServers(bool? isHealthy);
        public abstract Task<string?> GetServerPathByLobbyId(long lobbyId);
    }
}
