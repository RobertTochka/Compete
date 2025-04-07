using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_poco.Dto;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Services.UserRepository
{
    public abstract class CUserRepository: CRepository
    {
        public CUserRepository(ApplicationContext ctx) : base(ctx) { }
        public abstract void ClearRaitingCache(int page);
        public abstract Task<List<GetUserView>> GetUsersForAdmin(GetUsersForAdminRequest req);
        public abstract Task<GetUserDto?> GetUserAsync(long userId);
        public abstract Task<List<long>> GetUserIdsInTeam(long teamId);
        public abstract Task<User?> GetTrackingUserAsync(long userId);
        public abstract Task CreateUserAsync(User usr);
        public abstract Task<User> GetTrackingUserBySteamIdAsync(string steamId);
        public abstract Task<GetUserDto?> GetUserBySteamIdAsync(string steamId);
        public abstract Task<List<GetUserDto>> GetUsersFromContainerBySteamId(IEnumerable<string> neededSteamIds);
        public abstract Task<IDictionary<int, decimal>> Get_User_Profit_Stats_AtLastMonth_GroupedBy_ThreeDay
            (long userId);
        public abstract Task<UserSuperficialStats> GetSuperficialStatsAsync(long userId);
        public abstract Task<UserInfographicStats> GetUserInfographicStatsAsync(long userId);
        public abstract Task<GetUserRateDto[]> GetUsersInRaiting(int page, int? pageSize);
        public abstract Task<GetUserRateDto[]> GetUsersBatchForRefreshRaiting(int page, int pageSize,
            Expression<Func<User, object>> keySelector);
        public abstract Task<long> GetCurrentLobby(long userId);
        public abstract Task<UserArreasInfo> GetUserArreas(long userId);
        public abstract Task<UserAward?> GetUserUnprocessedAward();

        public abstract Task<int> GetCountsOfUsersByInterval(string? interval, bool? isOnline);
        public abstract Task<List<long>> GetPlatformAdminsSteamIds();
        public abstract Task<List<long>> GetPlatformAdminsUserIds();
        public abstract Task<bool> UserIsBanned(long userId);
        public abstract Task SetUserBanStatus(bool isBanned, long userId);
    }
}
