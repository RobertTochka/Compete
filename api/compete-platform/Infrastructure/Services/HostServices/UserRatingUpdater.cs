
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;

namespace compete_platform.Infrastructure.Services.HostServices
{
    public class UserRatingUpdater : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UserRatingUpdater> _logger;

        public UserRatingUpdater(IServiceProvider services, ILogger<UserRatingUpdater> logger) 
        {
            _services = services;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using var scope = _services.CreateScope();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        _logger.LogInformation("Start process of users raiting update");
                        await userService.UpdateUsersRaiting();
                        _logger.LogInformation("Process of raiting update completed");
                        AppConfig.LastTimeOfRatingUpdate = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Получил ошибку пока пытался обновить рейтинг пользователей:\n" +
                            $"{ex.Message}");
                    }
                    await Task.Delay((int)AppConfig.FrequencyOfRaitingUpdating.TotalSeconds * 1000);
                }
            });
       
    }
}
