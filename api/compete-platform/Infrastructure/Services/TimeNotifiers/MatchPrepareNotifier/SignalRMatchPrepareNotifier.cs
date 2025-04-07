namespace compete_poco.Infrastructure.Services.TimeNotifiers
{
    public class SignalRMatchPrepareNotifier : MatchPrepareNotifier
    {

        public SignalRMatchPrepareNotifier(IServiceProvider services, IActionTimeScheduler scheduler)
            : base(scheduler, services) { }

        protected override void ConfigureTimeoutEndFunction(StartNotifierInfo info, ActionTimeSchedulerParameters p)
        {
            var action = (object? input) => { return Task.CompletedTask; };
            p.Action = action;
        }
    }
}
