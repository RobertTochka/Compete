using compete_poco.Hubs;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using Microsoft.AspNetCore.SignalR;

namespace compete_platform.Infrastructure.Services
{
    public class SignalRLobbyChangingNotifier : ILobbyChangingNotifier
    {
        private readonly IHubContext<EventHub> _hubCtx;

        public SignalRLobbyChangingNotifier(IHubContext<EventHub> hubCtx) 
        { 
            _hubCtx = hubCtx;
        }
        public async Task NotifyUsersAboutLobbyChanges(ActionInfo action)
        {
            var usersInLobby = action.NewLobby.Teams.SelectMany(t => t.Users).Select(u => u.Id.ToString());
            await _hubCtx.Clients.Users(usersInLobby).SendAsync(nameof(IClientEventHub.LobbyChanged), action);
        }
    }
}
