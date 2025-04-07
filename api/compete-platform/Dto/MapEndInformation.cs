using compete_poco.Dto;
using compete_poco.Infrastructure.Services.LobbyService.Models;

namespace compete_platform.Dto
{
    public class MapEndInformation
    {
        public DateTime EndOfWarmup { get; set; }
        public ActionInfo NewLobby { get; set; } = null!;
    }
}
