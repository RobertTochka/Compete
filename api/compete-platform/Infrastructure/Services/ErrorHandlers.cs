using compete_poco.Dto;
using compete_poco.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace compete_platform.Infrastructure.Services
{
    public static class ErrorHandlers
    {
        public static async Task HandleEventHubError(string error, object hubAccess, IEnumerable<string>? userIds = null)
        {
            var userMessage = new Notification()
            {
                Type = NotificationType.Error,
                Message = error
            };
            if (hubAccess is IHubContext<EventHub> hub)
            {
                if (userIds is null)
                    throw new InvalidOperationException("Если передаете контекст хаба, требуется список пользователей");
                await hub.Clients
                        .Users(userIds).SendAsync(nameof(IClientEventHub.GetMessage), userMessage);

            }
            else if (hubAccess is EventHub eventHub)
            {
                IClientEventHub proxy;
                if (userIds is null)
                    proxy = eventHub.Clients.Caller;
                else
                    proxy = eventHub.Clients.Users(userIds);
                await proxy.GetMessage(userMessage);
            }
            else
                throw new InvalidProgramException("Передан неверный объект хаба");
        }
    }
}
