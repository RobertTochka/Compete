using compete_poco.Models;

namespace compete_poco.Infrastructure.Services.LobbyService.Models
{
    public class MapPickRequest
    {
        public long UserId { get; set; }
        public Map Map { get; set; }
        public long LobbyId { get; set; }
    }
}
