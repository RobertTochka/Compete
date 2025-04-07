using compete_poco.Models;

namespace compete_poco.Dto
{
    public class LobbyAdminConfiguration
    {
        public long Id { get; set; }
        public List<Map> PickMaps { get; set; } = null!;
        public bool Public { get; set; }
        public long ServerId { get; set; }
        public ServerConfig Config { get; set; } = null!;
        public Models.Type PlayersAmount { get; set;}
        public Format MatchFormat { get; set; }
    }
}
