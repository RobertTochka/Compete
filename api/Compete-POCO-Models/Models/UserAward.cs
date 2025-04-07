namespace compete_poco.Models
{
    public enum AwardType
    {
        MatchCanceled, Win, Lose
    }
    public class UserAward
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long LobbyId { get; set; }
        public Lobby? Lobby { get; set; }
        public DateTime PayTime { get; set; }
        public User? User { get; set; }
        public decimal Award { get; set; }
        public AwardType AwardType { get; set; }
    }
}
