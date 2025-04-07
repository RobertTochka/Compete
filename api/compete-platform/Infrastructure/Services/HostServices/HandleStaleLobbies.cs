
using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;

namespace compete_platform.Infrastructure.Services.HostServices
{
    public class HandleStaleLobbies : BackgroundService
    {
        private readonly IServiceProvider _services;
        /// <summary>
        /// Значение в течение которого лобби будут завершатся автоматически, если не было получено
        /// какой-либо актуальной информации от сервера
        /// (что лобби было отменено, карта сменена или логов со статистикой)
        /// Протухание лобби возникает, когда что-то идет неправильно
        /// </summary>
        private readonly TimeSpan _staleValue = TimeSpan.FromSeconds(
            AppConfig.MapInitialWarmupTimeGlobally.TotalSeconds * 1.5);

        public HandleStaleLobbies(IServiceProvider services) 
        {
            _services = services;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(async () =>
        {
            while(true)
            {
                //раз в минуту
                await Task.Delay(60 * 1000);
                using var scope = _services.CreateScope();
                var notifer = scope.ServiceProvider.GetRequiredService<ILobbyChangingNotifier>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<HandleStaleLobbies>>();
                logger.LogInformation($"Начинаю процедуру поиска протухшего лобби");
                var lobbySrc = scope.ServiceProvider.GetRequiredService<CLobbyRepository>();
                var lobbySrv = scope.ServiceProvider.GetRequiredService<ILobbyService>();
                var staledLobby = await lobbySrc.GetStaledLobby(_staleValue);
                if (staledLobby == default)
                {
                    logger.LogInformation("Протухших лобби не оказалось");
                    continue;
                }
                logger.LogWarning($"Протухшее лобби было найдено:[{staledLobby}].\n" +
                    $"Это сигналиизрует что обновления от игрового сервера не доходят и он" +
                    $"не может самостоятельно продолжить матч!");
                try
                {
                    logger.LogWarning($"Начинаю попытку завершить это стуъшее лобби {staledLobby}");
                    var action = await lobbySrv.Cancel_Lobby(staledLobby);
                    _ = notifer.NotifyUsersAboutLobbyChanges(action);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Попытка отменить стухшее лобби [{staledLobby}] провалилась\n" +
                        $"Это значит что один из слотов сервера занят впустую, нужно вмешательство\n" +
                        $"{ex.Message}");
                }
            }
        }, stoppingToken);
    }
}
