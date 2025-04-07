namespace compete_poco.Models
{
    public class AppealChatMessage
    {
        public long Id { get; set; }
        public string Content { get; set; } = null!;
        public long AppealChatId { get; set; }
        public AppealChat? AppealChat { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public DateTime SendTime { get; set; }
    }
}
