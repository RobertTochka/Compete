
using compete_poco.Infrastructure.Services;

namespace compete_platform.Infrastructure.Services.HostServices
{
    public class ProcessUserAwardsForMatches : BackgroundService
    {
        private readonly IServiceProvider _services;
        private  ILogger<ProcessUserAwardsForMatches> _logger;

        public ProcessUserAwardsForMatches(IServiceProvider services, ILogger<ProcessUserAwardsForMatches> logger) 
        {
            _services = services;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
           while(true)
            {
                _logger.LogInformation($"Начинаю обрабатывать награды для пользователей");
                while (true)
                {
                    
                    using var scope = _services.CreateScope();
                    var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
                    try
                    {
                        var result = await userSrv.HandleUnproccesedUserAward();
                        if (!result)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Получил ошибку при расчете награды пользователя \n {ex.Message}");
                    }
                }
                await Task.Delay(1000 * 10);
            }
        });
    }
}
