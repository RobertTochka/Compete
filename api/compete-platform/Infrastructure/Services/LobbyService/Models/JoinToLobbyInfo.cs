using Microsoft.AspNetCore.Mvc;

namespace compete_poco.Infrastructure.Services.LobbyService.Models
{
    public class JoinToLobbyInfo
    {
        [FromRoute]
        public long UserId { get; set; }
        [FromQuery]
        public long? InviterId { get; set; }
        [FromQuery]
        public long? TeamId { get; set; }
        [FromRoute]
        public long LobbyId { get; set; }
        [FromQuery]
        public Guid? Code { get; set; } = null;
    }
}
