using compete_platform.Dto;
using compete_poco.Infrastructure.Data;
using Polly.Contrib.WaitAndRetry;
using Polly;
using Polly.Retry;
using System.Text.Json;
using compete_poco.Infrastructure.Services;
using System.IO;
using Compete_POCO_Models.Infrastrcuture.Data;

namespace CompeteGameServerHandler.Infrastructure.Services.ServerRunner
{
    public class ServerRunner : IServerRunner
    {
        private readonly HttpClient _client;
        private readonly AppConfig _cfg;
        private readonly CServerRepository _serverRep;
        private readonly ILogger<ServerRunner> _logger;

        public AsyncRetryPolicy<HttpResponseMessage> PollyPolicyForHttp(ILogger logger) =>
             Policy.HandleResult<HttpResponseMessage>
                (res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogInformation($"Attempt {retryAttempt}: Retrying in {timespan.TotalSeconds} seconds due to {outcome.Result?.StatusCode}.");
                    });
        
        public ServerRunner(AppConfig cfg, 
            HttpClient client, 
            ILogger<ServerRunner> logger,
            CServerRepository serverRep)
        {
            _client = client;
            _cfg = cfg;
            _serverRep = serverRep;
            _logger = logger;
        }
        private void SetToken() => _client.DefaultRequestHeaders.Authorization = new("Bearer", _cfg.ServerManagingAccessKey);
        public async Task<ServerRunnerResponse?> GetServerOutput(string path, int port, long lobbyId)
            => await CreateCommand("output")(path, port, lobbyId);
        private Func<string, int, long, Task<ServerRunnerResponse?>> CreateCommand(string route)
        {
            return async (string path, int port, long lobbyId) =>
            {
                SetToken();
                var req = new ServerRunnerRequest() { LobbyId = lobbyId, Port = port };
                var stringReq = JsonSerializer.Serialize(req);
                var content = new StringContent(stringReq, System.Text.Encoding.UTF8, "application/json");
                var res = await PollyPolicyForHttp(_logger).ExecuteAsync(() => 
                _client.PostAsync($"http://{path}:{_cfg.CsServerManagingApiPort}/{route}", content));
                res.EnsureSuccessStatusCode();
                return await res.Content.ReadFromJsonAsync<ServerRunnerResponse>();
            };
        }
        public async Task<ServerRunnerResponse?> RunServer(string path, int port, long lobbyId)
        => await CreateCommand("start")(path, port, lobbyId);

        public async Task<ServerRunnerResponse?> StopServer(string path, int port, long lobbyId)
       => await CreateCommand("kill")(path, port, lobbyId);

        public async Task<bool> CheckServerHealthy(string path)
        {
            try
            {
                SetToken();
                var res = await _client.GetAsync($"http://{path}:{_cfg.CsServerManagingApiPort}/healthcheck");
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                _logger.LogWarning($"Сервер по пути {path} Был недоступен");
                return false;
            }
            return true;
        }
        public async Task<GetDemoFileResponse> GetDemoFile(GetDemoFileRequest req)
        {
            try
            {
                SetToken();
                var path = await _serverRep.GetServerPathByLobbyId(req.LobbyId) 
                    ?? throw new ArgumentNullException(nameof(req.LobbyId));
                var res = await _client.GetAsync($"http://{path}:{_cfg.CsServerManagingApiPort}/demo?" +
                    $"id={req.LobbyId}&mapname={req.Mapname}", HttpCompletionOption.ResponseHeadersRead);
                res.EnsureSuccessStatusCode();
                return new() 
                { 
                    Data = await res.Content.ReadAsStreamAsync(), 
                    Name = $"{req.LobbyId}_{req.Mapname}.dem" 
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning($"Не удалось скачать демо для сервера {req.LobbyId}");
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ApplicationException(AppDictionary.DemoFileNotFound);
                throw;
            }
        }
    }
}
