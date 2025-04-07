
using compete_poco.Infrastructure.Services.LobbyService.Models;

namespace compete_platform.Infrastructure.Services
{
    public interface ILobbyChangingNotifier
    {
        public Task NotifyUsersAboutLobbyChanges(ActionInfo action);
    }
}
