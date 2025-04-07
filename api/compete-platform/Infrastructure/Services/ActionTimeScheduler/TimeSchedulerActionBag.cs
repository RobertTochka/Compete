namespace compete_poco.Infrastructure.Services
{
    public class TimeSchedulerActionBag
    {
        public ActionTimeSchedulerParameters Parameters { get; set; } = null!;
        public Mutex Mutex { get; set; } = null!;
        public CancellationTokenSource CancellationTokenSource { get; set; } = null!;
        public Task CurrentTimer { get; set; } = null!;
    }
}
