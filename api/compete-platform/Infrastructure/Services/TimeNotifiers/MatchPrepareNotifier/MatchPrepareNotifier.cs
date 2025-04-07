
namespace compete_poco.Infrastructure.Services.TimeNotifiers
{
    public abstract class MatchPrepareNotifier : TimeNotifier
    {
        public MatchPrepareNotifier(IActionTimeScheduler scheduler, IServiceProvider services) : base(scheduler, services)
        {
        }
    }
}
