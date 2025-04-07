using compete_poco.Dto;

namespace compete_poco.Infrastructure.Services.ChatService
{
    public class SendMessageResponse
    {
        public List<long> UserIds { get; set; } = new List<long>();
        public GetChatMessageDto ChatMessage { get; set; } = new();
    }
}
