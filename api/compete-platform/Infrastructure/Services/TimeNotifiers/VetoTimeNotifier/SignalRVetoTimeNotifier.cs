using compete_poco.Hubs;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_platform.Infrastructure.Services.LobbyErrorHandler;

namespace compete_poco.Infrastructure.Services.TimeNotifiers
{
    public class SignalRVetoTimeNotifier : VetoTimeNotifier
    {

        public SignalRVetoTimeNotifier(IServiceProvider services, IActionTimeScheduler scheduler) : base(scheduler, services)
        { }

        protected override void ConfigureTimeoutEndFunction(StartNotifierInfo info, ActionTimeSchedulerParameters p)
        {
            var action = async (object? input) =>
            {
                var scope = _services.CreateScope();
                var lobbyHandler = scope.ServiceProvider.GetRequiredService<ILobbyErrorHandler>();
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<EventHub>>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<SignalRVetoTimeNotifier>>();
                var mapReq = input as MapPickRequest;
                if (mapReq is null)
                    throw new ArgumentNullException("Должен передаваться заранее сформированный обьект выбора карты по истечению времени");
                var lobby = scope.ServiceProvider.GetRequiredService<ILobbyService>();
                try
                {
                    var actualInfo = await lobby.DoAction(mapReq);
                    await hub.Clients.Users(info.UserIds.Select(u => u.ToString()))
                    .SendAsync(nameof(IClientEventHub.LobbyChanged), actualInfo);
                }
                catch(DbUpdateConcurrencyException)
                {
                }
                catch(Exception ex)
                {
                   await lobbyHandler.HandleVetoFailed(info.LobbyId, ex);
                }
               
            };
            p.Action = action;
        }
    }

}
