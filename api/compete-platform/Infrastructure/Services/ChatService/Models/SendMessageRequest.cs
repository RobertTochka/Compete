namespace compete_poco.Infrastructure.Services.ChatService
{
    public class SendMessageRequest
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; } = null!;
    }
}
