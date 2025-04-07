namespace compete_poco.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<ChatMessage>();
        }
        public long Id { get; set; }
        public Lobby? Lobby { get; set; }
        public Team? Team { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
