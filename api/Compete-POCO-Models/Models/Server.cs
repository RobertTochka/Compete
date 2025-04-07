namespace compete_poco.Models
{
    public class LobbyPort 
    {
        public override int GetHashCode()
        {
            return Port;
        }
        public long LobbyId { get; set; }
        public int Port { get; set; }
    }
    public class Server
    {
        public Server()
        {
            Lobbies = new List<Lobby>();
            PlayingPorts = new();
        }
        public int Id { get; set; }
        public string Location { get; set; } = null!;
        public string Path { get; set; } = null!;
        public List<LobbyPort> PlayingPorts { get; set; }
        public bool IsHealthy { get; set; } = true;
        public ICollection<Lobby> Lobbies { get; set; }
    }
}
