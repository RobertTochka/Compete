namespace compete_poco.Models
{
    public class AppealChat
    {
        public long Id { get; set; }
        public List<long> UserIds { get; set; } = new List<long>();
        public bool IsRead { get; set; } = new ();
        public ICollection<AppealChatMessage> Messages { get; set; } = new List<AppealChatMessage>();
    }
}
