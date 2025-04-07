using compete_poco.Dto;

namespace compete_poco.Infrastructure.Services.LobbyService.Models
{
    public class ActionInfo
    {
        public GetLobbyDto NewLobby { get; set; } = new();
        public bool IsPickNow { get; set; }
        public long NextPickUserId { get; set;}
        public bool PickingComplete { get;set; }
    }
}
