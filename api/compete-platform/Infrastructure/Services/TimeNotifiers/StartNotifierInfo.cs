using compete_poco.Infrastructure.Services.LobbyService.Models;

namespace compete_poco.Infrastructure.Services
{
    public class StartNotifierInfo
    {
       public int AvailableSeconds { get; set; }
        public List<long> UserIds { get; set; } = new();
        public long LobbyId { get;set; }
        public object Input { get; set; } = new();
    }
}
