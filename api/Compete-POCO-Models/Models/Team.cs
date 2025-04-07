namespace compete_poco.Models
{
    public class Team
    {
        public Team() 
        { 
            Users = new List<User>();
            WonMatches = new List<Match>();
        }
        public long Id { get; set; }
        public long CreatorId { get; set; }
        public long LobbyId { get; set; }
        public Lobby? Lobby { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; }
        public ICollection<Match> WonMatches { get; set; }
        public long ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
