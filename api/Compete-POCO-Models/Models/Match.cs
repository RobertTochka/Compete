namespace compete_poco.Models
{
    public class Match
    {
        public Match()
        {
            Stats = new List<UserStat>();
        }
        public long Id { get; set; }
        public long? TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<UserStat> Stats { get; set; }
        public long LobbyId { get; set; }
        public short FirstTeamScore { get; set; }
        public short SecondTeamScore { get; set; }
        public Lobby? Lobby { get; set; }
        public Map PlayedMap { get; set; }
    }
}
