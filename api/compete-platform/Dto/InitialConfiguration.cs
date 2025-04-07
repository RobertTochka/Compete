using compete_poco.Models;

namespace CompeteGameServerHandler.Dto
{
    public class MapInformation
    {
        public long TeamId { get; set; }
        public bool IsPicked { get; set; }
        public string Map { get; set; } = null!;
    }
    public class TeamInformation
    {
        public long Id { get; set; }
        public long CreatorSteamId { get; set; }
        public List<long> SteamIds { get; set; } = new();
    }
    public enum Type
    {
        v1 = 1, v2 = 2, v3 = 3, v4 = 4, v5 = 5
    }
    public class InitialConfiguration
    {
        public long Id { get; set; }
        public Type PlayersAmount { get; set; } = Type.v5;
        public ServerConfig Cfg { get; set; } = null!;
        public List<MapInformation> PlayingMaps { get; set; } = new();
        public List<TeamInformation> Teams { get; set; } = new();
        public DateTime EdgeConnectTimeOnFirstMap { get; set; }
        public TimeSpan WaitToStartTime { get; set; } = TimeSpan.FromSeconds(240);
        public TimeSpan StopServerTime { get; set; } = TimeSpan.FromSeconds(40);
        public List<long> AdminsId { get; set; } = new();
    }
}
