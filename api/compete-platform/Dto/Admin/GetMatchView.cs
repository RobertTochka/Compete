using compete_poco.Dto;
using compete_poco.Models;

namespace compete_platform.Dto.Admin
{
    public class GetMatchView
    {
        public long Id { get; set; }
        public GetServerDto Server { get; set; } = null!;
        public LobbyStatus Status { get; set; }
        public int? Port { get; set; }
        public compete_poco.Models.Type PlayersAmount { get; set; }
        public object MapActions { get; set; } = new();
    }
}
