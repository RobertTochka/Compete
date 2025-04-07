namespace compete_poco.Models
{
    public class UserBid
    {
        public long Id { get; set; }
        public decimal Bid { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public long LobbyId { get; set; }
        public Lobby? Lobby { get; set; }
    }
}
