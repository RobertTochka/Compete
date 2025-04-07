using Compete_POCO_Models.EventVisitors;
using Compete_POCO_Models.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace compete_poco.Models
{
    public enum Format
    {
        BO1 = 1, BO3 = 2, BO5 = 3
    }
    public enum Type
    {
        v1 = 1, v2 = 2, v3 = 3, v4 = 4, v5 = 5
    }
    public enum LobbyStatus
    {
        Configuring, Veto, Playing, Canceled, Over, Warmup
    }
    public class Lobby : IContainsValuableEvents
    {
        public Lobby()
        {
            Teams = new List<Team>();
            Bids = new List<UserBid>();
            Matches = new List<Match>();
            PickMaps = new List<Map>();
            Awards = new List<UserAward>();
            MapActions = new List<MapActionInfo>();
        }
        public long Id { get; set; }
        public ICollection<Match> Matches { get; set; }
        public List<Map> PickMaps { get; set; }
        public long CreatorId { get; set; }
        public User? Creator { get; set; }
        public bool Public { get; set; }
        public Guid CodeToConnect { get; set; }
        public ICollection<UserBid> Bids { get; set; }
        public ICollection<UserAward> Awards { get; set; }
        public int ServerId { get; set; }
        public Server? Server { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ServerConfig Config { get; set; } = new();
        public LobbyStatus Status { get; set; } = LobbyStatus.Configuring;
        public long ChatId { get; set; }
        public Chat? Chat { get; set; }
        public long? TeamWinner { get; set; }
        public DateTime? LastServerUpdate { get; set; }
        [NotMapped]
        public IEventVisitor<Lobby> EventVisitor { get; set; } = new DefaultEventVisitor();
        public Type PlayersAmount { get; set; } = Type.v5;
        public Format MatchFormat { get; set; } = Format.BO1;
        public List<MapActionInfo> MapActions { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EdgeConnectTimeOnFirstMap { get; set; }
        public TimeSpan WaitToStartTime { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }

        public string? GetEventPayload() => EventVisitor.Visit(this);
    }
}
