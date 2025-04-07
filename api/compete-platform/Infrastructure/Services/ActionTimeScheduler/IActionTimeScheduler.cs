

namespace compete_poco.Infrastructure.Services
{
    public interface IActionTimeScheduler
    {
        public Task StartNotifyAboutTime(ActionTimeSchedulerParameters parameters);
        public void RefreshNotifyingAboutTime(ActionTimeSchedulerParameters refreshParameters);
        public Task StopNotifyingAboutTime(int id);
    }
}
