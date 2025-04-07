using compete_poco.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace compete_poco.Infrastructure.Services
{
    public abstract class TimeNotifier
    {
        protected int AvailableSeconds { get; set; } = 0;
        protected readonly IActionTimeScheduler _scheduler;
        protected readonly IServiceProvider _services;

        public TimeNotifier(IActionTimeScheduler scheduler, IServiceProvider services)
        {
            _scheduler = scheduler;
            _services = services;
        }
        protected Func<int, Task> GetDefaultClientSecondTimeNotifier(StartNotifierInfo info) => async (int seconds) =>
        {
            var scope = _services.CreateScope();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<EventHub>>();
            await hub.Clients.Users(info.UserIds.Select(u => u.ToString()))
            .SendAsync(nameof(IClientEventHub.NotifyAboutAvailablePickMapSeconds), seconds);
        };
        public virtual async Task StartNotifyAboutTime(StartNotifierInfo info)
        {
            AvailableSeconds = info.AvailableSeconds;
            
            var actionTimerParameters = new ActionTimeSchedulerParameters()
            {
                EverysecondAction = GetDefaultClientSecondTimeNotifier(info),
                AvailableSeconds = info.AvailableSeconds,
                Id = (int)info.LobbyId,
                Input = info.Input
            };
            ConfigureTimeoutEndFunction(info, actionTimerParameters);
            await _scheduler.StartNotifyAboutTime(actionTimerParameters);
        }
        protected abstract void ConfigureTimeoutEndFunction(StartNotifierInfo info, ActionTimeSchedulerParameters p);
        public virtual void RefreshNotifyingAboutTime(long lobbyId, object input) =>
        _scheduler.RefreshNotifyingAboutTime(new() { AvailableSeconds = AvailableSeconds, Input = input, Id = (int)lobbyId });
        public virtual async Task StopNotifyingAboutTime(long lobbyId) => await _scheduler.StopNotifyingAboutTime((int)lobbyId);
    }
}
