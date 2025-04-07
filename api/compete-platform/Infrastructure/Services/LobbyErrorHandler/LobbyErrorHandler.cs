
using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Dto;
using compete_poco.Hubs;
using compete_poco.Infrastructure.Services;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.SignalR;

namespace compete_platform.Infrastructure.Services.LobbyErrorHandler
{
    public class LobbyErrorHandler : ILobbyErrorHandler
    {
        private readonly IHubContext<EventHub> _hub;
        private readonly ILogger<LobbyErrorHandler> _logger;
        private readonly ILobbyService _lobbyServ;
        private readonly CLobbyRepository _rep;

        public LobbyErrorHandler(IHubContext<EventHub> hub, 
            ILogger<LobbyErrorHandler> logger, 
            ILobbyService lobbyServ,
            CLobbyRepository rep) 
        {
            _hub = hub;
            _logger = logger;
            _lobbyServ = lobbyServ;
            _rep = rep;
        }
        public async Task HandleVetoFailed(long lobbyId, Exception ex)
        {
            var userIds = (await _rep.GetUsersIdInLobbyAsync(lobbyId)).Select(t => t.ToString());
            _logger.LogError($"На сервере возникла критическая ошибка для всех участников лобби - [{lobbyId}]\n" +
                $"{ex.Message}");
            await ErrorHandlers.HandleEventHubError(AppDictionary.ServerErrorOcurred, _hub, userIds);
            _logger.LogError($"Lobby {lobbyId} was failed on Veto");
            var lobbyAction = await _lobbyServ.ResetLobbyAfterFailedVeto(lobbyId);
            IClientProxy players = _hub.Clients
                        .Users(userIds);
            await players.SendAsync(nameof(IClientEventHub.LobbyChanged), lobbyAction);
            await players.SendAsync(nameof(IClientEventHub.GetMessage),
                        new Notification() { Type = NotificationType.Error, Message = AppDictionary.LobbyWasResetted });
            _logger.LogWarning($"Лобби {lobbyId} было перезапущено");
        }
    }
}
