using compete_poco.Infrastructure.Services.ChatService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace compete_poco.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatProvider;
        private long GetUserId() => long.Parse(User.Claims.First(c => c.Type.Equals("Id")).Value);
        public ChatController(IChatService chatProvider) 
        {
            _chatProvider = chatProvider; 
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages(int page, int pageSize, long chatId)
        {
            var messagesRequest = new GetMessagesRequest { Page = page, PageSize = pageSize, UserId = GetUserId(), ChatId = chatId };
            var messages = await _chatProvider.GetMessages(messagesRequest);
            return Ok(messages);
        }
        [HttpGet("appeals/{chatId}")]
        public async Task<IActionResult> GetAppealMessages(int page, int pageSize, long chatId)
        {
            var messagesRequest = new GetMessagesRequest { Page = page, PageSize = pageSize, UserId = GetUserId(), ChatId = chatId };
            var messages = await _chatProvider.GetAppealMessages(messagesRequest);
            return Ok(messages);
        }
        [HttpGet("appealchat/{userId}")]
        public async Task<IActionResult> GetAppealChatByUserId(long userId)
        {
            var chatId = await _chatProvider.GetAppealChatByUserId(userId);
            return Ok(chatId);
        }
        [HttpGet]
        public async Task<IActionResult> GetCommonChatId()
        {
            var res = await _chatProvider.GetCommonChatId();
            return Ok(res);
        }
    }
}
