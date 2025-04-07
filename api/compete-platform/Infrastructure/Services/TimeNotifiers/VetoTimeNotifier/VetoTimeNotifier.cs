
namespace compete_poco.Infrastructure.Services.TimeNotifiers
{
    public abstract class VetoTimeNotifier : TimeNotifier
    {
        protected VetoTimeNotifier(IActionTimeScheduler scheduler, IServiceProvider services) : base(scheduler, services)
        {}
    }
}
