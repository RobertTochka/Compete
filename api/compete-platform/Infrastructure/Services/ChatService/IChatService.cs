using compete_platform.Dto;
using compete_poco.Dto;
using compete_poco.Models;

namespace compete_poco.Infrastructure.Services.ChatService
{
    public interface IChatService
    {
        public Task<List<GetChatMessageDto>> GetMessages(GetMessagesRequest req);
        public Task<List<GetChatMessageDto>> GetAppealMessages(GetMessagesRequest req);
        public Task<long> GetAppealChatByUserId(long userId);
        public Task<SendMessageResponse> SendMessage(SendMessageRequest req);
        public Task<SendMessageResponse> SendAppealMessage(SendMessageRequest req);
        public Task<List<AppealChat>> GetAppealChats(GetAppealChatsDto req);
        public Task SetAppealChatRead(long chatId);
        public Task<long> GetCommonChatId();
    }
}
