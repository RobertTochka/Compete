namespace compete_poco.Infrastructure.Services.ChatService
{
    public class GetMessagesRequest
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public bool IsAppeal { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
