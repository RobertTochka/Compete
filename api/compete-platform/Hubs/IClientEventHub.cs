using compete_poco.Dto;
using compete_poco.Infrastructure.Services.LobbyService.Models;

namespace compete_poco.Hubs
{
    public interface IClientEventHub
    {
        public Task LobbyChanged(ActionInfo info);
        public Task GetMessage(Notification note);
        public Task GetInvite(JoinToLobbyInfo invite);
        public Task GetChatMessage(GetChatMessageDto message);
        public Task NotifyAboutAvailablePickMapSeconds(int seconds);
    }
}
