namespace compete_poco.Models
{
    public class UserStat
    {
        public long Id { get; set; }
        public long MatchId { get; set; }
        public Match? Match { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int Headshots { get; set; }
    }
}
