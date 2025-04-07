

namespace compete_poco.Dto
{
    public class GetChatMessageDto
    {
        public long Id { get; set; }
        public string Content { get; set; } = null!;
        public GetUserDto User { get; set; } = null!;
        public long ChatId { get;set; }
        public DateTime SendTime { get; set; }
        
    }
}
