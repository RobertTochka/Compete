using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.LobbyErrorHandler;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Dto;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.ChatService;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace compete_poco.Hubs
{
    [Authorize]
    public class EventHub : Hub<IClientEventHub>
    {
        private readonly ILobbyService _lobbyProvider;
        private readonly IUserService _userProvider;
        private readonly IChatService _chatProvider;
        private readonly ILobbyErrorHandler _lobbyHandler;
        private readonly ILogger<EventHub> _logger;
        public EventHub(ILobbyService lobbyProvider,
            IUserService userProvider,
            IChatService chatProvider,
            ILogger<EventHub> logger,
            ILobbyErrorHandler lobbyHandler)
        {
            _lobbyProvider = lobbyProvider;
            _userProvider = userProvider;
            _chatProvider = chatProvider;
            _lobbyHandler = lobbyHandler;
            _logger = logger;
        }
        private async Task HandleErrorManually(Func<Task> handler, Func<Exception, Task> onError)
        {
            try
            {
                await handler();
            }
            catch (LobbySmoothlyError ex)
            {
                await ErrorHandlers.HandleEventHubError(ex.Message, this);
            }
            catch (Exception ex)
            {
                await onError(ex);
            }
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                await _userProvider.SetUserAvailability(UserId, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Не удалось обновить статус пользователя {UserId} для сети\n" +
                    $"{ex.Message}");
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                await _userProvider.SetUserAvailability(UserId, false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Не удалось обновить статус пользователя {UserId} для оффлайна\n" +
                    $"{ex.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }
        private long UserId => long.Parse(Context.UserIdentifier!);
        public async Task SendLobbyChange(LobbyAdminConfiguration lobby)
        {
            var newLobby = await _lobbyProvider.SetNewLobbyConfiguration(lobby, UserId);
            await NotifyAboutLobbyChanges(newLobby);
        }
        public async Task ChangeUserBid(decimal bid)
        {
            var lobby = await _lobbyProvider.ChangeUserBid(new() { UserId = UserId, Bid = bid });
            await NotifyAboutLobbyChanges(lobby);
        }
        private async Task NotifyAboutLobbyChanges(ActionInfo info)
        {
            var userIds = info.NewLobby.Teams.SelectMany(t => t.Users).Select(u => u.Id.ToString());
            await Clients.Users(userIds).LobbyChanged(info);
        }
        public async Task ChangeTeamName(string teamName)
        {
            var newLobby = await _lobbyProvider.ChangeTeamName(new() { UserId = UserId, NewName = teamName });
            await NotifyAboutLobbyChanges(newLobby);
        }
        public async Task SendInvite(long userId)
        {
            var invite = await _lobbyProvider.CreateInviteForUser(new() { UserId = userId, InviterId = UserId });
            await Clients.Caller.GetMessage(new() { Type = NotificationType.Info, Message = "Приглашение отправлено" });
            await Clients.User(userId.ToString()).GetInvite(invite);
        }
        public async Task SendMessage(MessageRequest msgReq)
        {
            var req = new SendMessageRequest() { Message = msgReq.Message, UserId = UserId, ChatId = msgReq.ChatId };
            var res = (await _chatProvider.SendMessage(req));
            if (res.UserIds.Any())
                await Clients.Users(res.UserIds.Select(u => u.ToString())).GetChatMessage(res.ChatMessage);
            else
                await Clients.All.GetChatMessage(res.ChatMessage);

        }
        public async Task SendAppealMessage(MessageRequest msgReq)
        {
            var req = new SendMessageRequest() { Message = msgReq.Message, UserId = UserId, ChatId = msgReq.ChatId };
            var res = await _chatProvider.SendAppealMessage(req);
            if (res.UserIds.Any())
                await Clients.Users(res.UserIds.Select(u => u.ToString())).GetChatMessage(res.ChatMessage);
            else
                await Clients.All.GetChatMessage(res.ChatMessage);
        }
        public async Task SetAppealChatRead(long chatId)
        {
            await _chatProvider.SetAppealChatRead(chatId);
        }
        public async Task StartVeto(long lobbyId)
        {
            var newLobby = await _lobbyProvider.StartMapPick(lobbyId, UserId);
            await NotifyAboutLobbyChanges(newLobby);
        }
        public async Task SelectMap(Map map, long lobbyId)
        {
            var f = async () =>
            {
                MapPickRequest req = new() { Map = map, UserId = UserId, LobbyId = lobbyId };
                var info = await _lobbyProvider.DoAction(req);
                await NotifyAboutLobbyChanges(info);
            };
            var onError = async (Exception ex) =>
                await _lobbyHandler.HandleVetoFailed(lobbyId, ex);
            await HandleErrorManually(f, onError);
        }
    }
}
