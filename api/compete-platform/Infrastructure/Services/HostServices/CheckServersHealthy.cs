
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;
using CompeteGameServerHandler.Infrastructure.Services;
using CompeteGameServerHandler.Infrastructure.Services.ServerRunner;

namespace compete_platform.Infrastructure.Services.HostServices
{
    public class CheckServersHealthy : BackgroundService
    {
        private IServiceProvider _services;

        public CheckServersHealthy(IServiceProvider services) 
        { 
            _services =  services;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.Run(async () =>
            {
                while (true)
                {
                  
                    using var scope = _services.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CheckServersHealthy>>();
                    logger.LogInformation("Start proccessing of checking servers healthy");
                    var serverSrc = scope.ServiceProvider.GetRequiredService<CServerRepository>();
                    var serverRunner = scope.ServiceProvider.GetRequiredService<IServerRunner>();
                    try
                    {
                        var servers = await serverSrc.GetAllServers();
                        var serversHealthyResultTasks = servers.Select(s =>
                        (s.Path, serverRunner.CheckServerHealthy(s.Path))).ToList();
                        await Task.WhenAll(serversHealthyResultTasks.Select(s => s.Item2));
                        var servserversHealthyResult = serversHealthyResultTasks
                        .Where(s => s.Item2.Status == TaskStatus.RanToCompletion)
                        .Select(s => (s.Path, s.Item2.Result)).ToList();
                        foreach (var r in servserversHealthyResult) {
                            logger.LogInformation($"Server:  {r}");
                            await serverSrc.UpdateServerHealthyStatus(r.Path, r.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("During the checking servers healthy process error ocurred\n" +
                            $"{ex.Message}");
                    }
                    logger.LogInformation("Process of checking servers healthy ended");
                    await Task.Delay((int)AppConfig.FrequencyOfServersHealthyChecking.TotalMilliseconds);
                }
            });
       
    }
}
