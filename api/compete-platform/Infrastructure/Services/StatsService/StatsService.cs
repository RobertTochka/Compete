using compete_platform.Dto.Admin;
using compete_platform.Infrastructure.Services.PayEventsRepository;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.UserRepository;

namespace compete_platform.Infrastructure.Services
{
    public class StatsService : IStatsService
    {
        private readonly CServerRepository _serverSrc;
        private readonly CUserRepository _userSrc;
        private readonly CLobbyRepository _lobbySrc;
        private readonly CPayEventsRepository _payEventsSrc;

        public StatsService(CLobbyRepository lobbySrc,
            CUserRepository userSrc,
            CPayEventsRepository payEventsSrc,
            CServerRepository serverSrc)
        {
            _serverSrc = serverSrc;
            _userSrc = userSrc;
            _lobbySrc = lobbySrc;
            _payEventsSrc = payEventsSrc;
        }
        public async Task<GetAdminStatsResponse> GetAdminStats(GetAdminStatsRequest request)
        {
            var financialRotationsTask = _payEventsSrc
                .GetFinancialRotationByInterval(request.FinancialRotationInterval);
            var matchesCountTask = _lobbySrc.GetLobbyCounts(request.MatchesStatus);
            var usersCountTask = _userSrc.GetCountsOfUsersByInterval(request.UserInterval, request.IsOnlineUsers);
            var lobbyComissionTask = _lobbySrc.GetLobbyComissions(request.LobbyComissionsInterval);
            var serversCountTask = _serverSrc.GetCountsOfTypedServers(request.OnlyHealthyServers);
            await Task.WhenAll(
                financialRotationsTask,
                matchesCountTask, 
                usersCountTask, 
                lobbyComissionTask, 
                serversCountTask);
            return new()
            {
                FinancialRotation = financialRotationsTask.Result,
                UsersCount = usersCountTask.Result,
                LobbyComissions = lobbyComissionTask.Result,
                ServersCount = serversCountTask.Result,
                MatchesCount = matchesCountTask.Result,
            };
        }
    }
}
