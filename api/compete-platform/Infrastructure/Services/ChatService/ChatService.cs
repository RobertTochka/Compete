using AutoMapper;
using AutoMapper.QueryableExtensions;
using compete_platform.Dto;
using compete_platform.Dto.Common;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Dto;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace compete_poco.Infrastructure.Services.ChatService
{
    public class ChatService : IChatService
    {
        private IUserService _userProvider;
        private readonly ApplicationContext _ctx;
        private readonly CLobbyRepository _lobbySrc;
        private readonly CUserRepository _userSrc;
        private readonly IMapper _mapper;
        private readonly ILobbyService _lobbyProvider;

        public ChatService(IUserService userProvider, 
            ApplicationContext ctx, IMapper mapper, 
            ILobbyService lobbyProvider,
            CLobbyRepository lobbySrc,
            CUserRepository userSrc)
        {
            _userProvider = userProvider;
            _ctx = ctx;
            _lobbySrc = lobbySrc;
            _userSrc = userSrc;
            _mapper = mapper;
            _lobbyProvider = lobbyProvider;
        }

        public async Task<List<GetChatMessageDto>> GetMessages(GetMessagesRequest req)
        {
            var chat = await _ctx.Chats
                .Include(c => c.Messages
                    .OrderByDescending(cm => cm.SendTime)
                    .Skip((req.Page -1) * req.PageSize)
                    .Take(req.PageSize))
                .ThenInclude(cm => cm.User)
                .Include(c => c.Lobby)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.Id.Equals(req.ChatId));
            if (chat is null)
                throw new ApplicationException(AppDictionary.ChatNotExist);
            await CheckUserPermissionForSend(chat, req.UserId);
           
            var mapped = _mapper.Map<List<GetChatMessageDto>>(chat.Messages);
            var usersForMap = mapped.Select(m => (ISteamUserBasedDto<GetUserDto>)m.User).ToList();
            if(usersForMap.Any())
                await _userProvider.LinkUsersToSteam(usersForMap);
            return mapped;
        }
        public async Task<List<GetChatMessageDto>> GetAppealMessages(GetMessagesRequest req)
        {            
            var chat = await _ctx.AppealChats
                .FirstOrDefaultAsync(c => c.Id.Equals(req.ChatId));

            if (chat is null)
                throw new ApplicationException(AppDictionary.ChatNotExist);

            var messages = await _ctx.AppealChatMessages
                .Where(m => m.AppealChatId == chat.Id)
                .OrderByDescending(m => m.SendTime)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Include(m => m.User)
                .ToListAsync();

            var mapped = _mapper.Map<List<GetChatMessageDto>>(messages);
            var usersForMap = mapped.Select(m => (ISteamUserBasedDto<GetUserDto>)m.User).ToList();
            if(usersForMap.Any())
                await _userProvider.LinkUsersToSteam(usersForMap);
            return mapped;
        }
        public async Task<long> GetAppealChatByUserId(long userId)
        {
            var chat = await _ctx.AppealChats
                .FirstOrDefaultAsync(c => c.UserIds.Contains(userId));

            if (chat is null)
                throw new ApplicationException(AppDictionary.ChatNotExist);

            return chat.Id;
        }
        private async Task CheckUserPermissionForSend(Chat chat, long userId)
        {
            if (chat.Lobby is not null)
            {
                var lobbyUsersTask =  _lobbySrc.GetUsersIdInLobbyAsync(chat.Lobby.Id);
                var adminsTask = _userSrc.GetPlatformAdminsUserIds();
                await Task.WhenAll(lobbyUsersTask, adminsTask);
                if (!lobbyUsersTask.Result.Contains(userId) && !adminsTask.Result.Contains(userId))
                    throw new ApplicationException(AppDictionary.PermissionDenied);
            }
            else if (chat.Team is not null)
            {
                var teamUsersTask = _userSrc.GetUserIdsInTeam(chat.Team.Id);
                var adminsTask = _userSrc.GetPlatformAdminsUserIds();
                await Task.WhenAll(teamUsersTask, adminsTask);
                if (!teamUsersTask.Result.Contains(userId) && !adminsTask.Result.Contains(userId))
                    throw new ApplicationException(AppDictionary.PermissionDenied);
            }
        }
        private async Task<List<long>> GetChatAudience(Chat chat, long userId)
        {
            if (chat.Lobby is not null)
                return await _lobbySrc.GetUsersIdInLobbyAsync(chat.Lobby.Id);
            else if(chat.Team is not null)
                return await _userSrc.GetUserIdsInTeam(chat.Team.Id);
            return Enumerable.Range(0, 0).Select(t => (long)t).ToList();
        }
        private void ValidateMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                throw new ApplicationException(AppDictionary.MessageIsEmpty);
        }
        public async Task<SendMessageResponse> SendMessage(SendMessageRequest req)
        {
            ValidateMessage(req.Message);
            var chat = await _ctx.Chats
                .Include(c => c.Lobby)
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.Id.
                    Equals(req.ChatId));
            if (chat is null)
                throw new ApplicationException(AppDictionary.ChatNotExist);
            await CheckUserPermissionForSend(chat, req.UserId);
            var msg = new ChatMessage() 
            { 
                ChatId = chat.Id, 
                SendTime = DateTime.UtcNow, 
                UserId = req.UserId, 
                Content = req.Message 
            };
            await _ctx.ChatMessages.AddAsync(msg);
            await _ctx.SaveChangesAsync();
            var user = await _userProvider.GetUserAsync(new() { IncludeFriends = false, UserId = req.UserId });
            var chatAudience = await GetChatAudience(chat, req.UserId);
            var chatDto = _mapper.Map<GetChatMessageDto>(msg);
            chatDto.User = user;
            chatDto.ChatId = req.ChatId;
            return new() { UserIds = chatAudience, ChatMessage =  chatDto };
        }
        public async Task<SendMessageResponse> SendAppealMessage(SendMessageRequest req)
        {
            ValidateMessage(req.Message);

            var appealChat = await _ctx.AppealChats
                .FirstOrDefaultAsync(c => c.Id == req.ChatId);

            if (appealChat is null)
            {
                appealChat = await CreateAdminChatIfNotExists(req.UserId);
            }

            var msg = new AppealChatMessage()
            {
                AppealChatId = appealChat.Id,
                SendTime = DateTime.UtcNow,
                UserId = req.UserId,
                Content = req.Message
            };

            var user = await _userProvider.GetUserAsync(new() { IncludeFriends = false, UserId = req.UserId });
            if (!user.IsAdmin) 
            {
                appealChat.IsRead = false;
            }
            await _ctx.AppealChatMessages.AddAsync(msg);
            await _ctx.SaveChangesAsync();

            var chatAudience = appealChat.UserIds ?? new List<long>();

            var chatDto = _mapper.Map<GetChatMessageDto>(msg);
            chatDto.User = user;
            chatDto.ChatId = appealChat.Id;

            return new SendMessageResponse { UserIds = chatAudience, ChatMessage = chatDto };
        }

        private async Task<AppealChat> CreateAdminChatIfNotExists(long userId)
        {
            var existingChat = await _ctx.AppealChats
                .FirstOrDefaultAsync(c => c.UserIds.Contains(userId));

            if (existingChat is not null)
                return existingChat;

            var adminIds = await _userSrc.GetPlatformAdminsUserIds();
    
            if (!adminIds.Any())
                throw new ApplicationException("Нет доступных администраторов для создания чата.");

            var userExists = await _ctx.Users.AnyAsync(u => u.Id == userId);
    
            if (!userExists)
                throw new ApplicationException("Пользователь не найден.");

            var newChat = new AppealChat
            {
                UserIds = new List<long> { userId }.Concat(adminIds).ToList()
            };

            _ctx.AppealChats.Add(newChat);
            await _ctx.SaveChangesAsync();

            return newChat;
        }
        public async Task<List<AppealChat>> GetAppealChats(GetAppealChatsDto req)
        {
            long idSearchParam = default;
            _ = long.TryParse(req.searchParams, out idSearchParam);

            var chats = await _ctx.AppealChats
                .OrderBy(c => c.IsRead)
                .ToListAsync();

            if (!string.IsNullOrEmpty(req.searchParams))
            {
                if (idSearchParam != default)
                {
                    chats = chats.Where(c => c.UserIds.Contains(idSearchParam)).ToList();
                }
                else
                {
                    var allUserIds = chats.SelectMany(c => c.UserIds).Distinct().ToList();

                    var matchedUsers = await _ctx.Users
                        .Where(u => allUserIds.Contains(u.Id) &&
                                    u.Name.ToLower().Contains(req.searchParams.ToLower()))
                        .Select(u => u.Id)
                        .ToListAsync();

                    chats = chats.Where(c => c.UserIds.Any(uid => matchedUsers.Contains(uid))).ToList();
                }
            }

            return chats
                .Skip((req.page - 1) * req.pageSize)
                .Take(req.pageSize)
                .ToList();
        }
        public async Task SetAppealChatRead(long chatId)
        {
            var appealChat = await _ctx.AppealChats
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (appealChat is null) return;

            appealChat.IsRead = true;
            await _ctx.SaveChangesAsync();
        }

        public async Task<long> GetCommonChatId()
        {
            var result = await _ctx.Chats
                .Where(c => c.Team == null && c.Lobby == null)
                .Select(c => c.Id)
                .FirstAsync();
            return result;
        }
    }
}
