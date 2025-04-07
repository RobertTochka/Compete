namespace compete_poco.Models
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string Content { get; set; } = null!;
        public long ChatId { get; set; }
        public Chat? Chat { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public DateTime SendTime { get; set; }
    }
}
