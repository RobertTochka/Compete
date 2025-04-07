using Compete_POCO_Models;
using Compete_POCO_Models.EventVisitors;
using Compete_POCO_Models.Models;
using System.ComponentModel.DataAnnotations.Schema;


namespace compete_poco.Models;

public class User : IContainsValuableEvents
{
    public User()
    {
        Bids = new List<UserBid>();
        Stats = new List<UserStat>();
        Awards = new List<UserAward>();
        Teams = new List<Team>();
        OwnedLobbies = new List<Lobby>();
        Pays = new List<Pay>();
        Reports = new List<Report>();
    }
    [NotMapped]
    public IEventVisitor<User> EventVisitor { get; set; } = new DefaultEventVisitor();
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string SteamId { get; set; } = null!;
    public ICollection<Lobby> OwnedLobbies { get; set; }
    public decimal Balance { get; set; }
    public double Rate { get; set; }
    public long? RatePlace { get; set; }
    public DateTime RegistrationDate { get; set; }
    public UserRole Role { get; set; }
    public ICollection<UserBid> Bids { get; set; }
    public ICollection<UserAward> Awards { get; set; }
    public ICollection<UserStat> Stats { get;set; }
    public ICollection<Team> Teams { get; set; }
    public ICollection<Pay> Pays { get; set; }
    public ICollection<Report> Reports { get; set; }
    public bool IsOnline { get; set; }
    public bool IsAdmin { get;set; }
    public bool IsBanned { get; set; } = false;
    public DateTime LastSteamInfoUpdate {  get; set; }
    public DateTime LastEnter {  get; set; }

    public string? GetEventPayload() => EventVisitor.Visit(this);
}
