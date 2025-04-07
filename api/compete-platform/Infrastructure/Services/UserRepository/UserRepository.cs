using AutoMapper;
using AutoMapper.QueryableExtensions;
using compete_platform;
using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_platform.Infrastructure.ValueResolvers;
using compete_poco.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Services.UserRepository
{
    public class UserRepository : CUserRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _factory;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        public UserRepository(ApplicationContext ctx,
            IDbContextFactory<ApplicationContext> dbContextFactory,
            IMapper mapper,
            IDistributedCache cache) : base(ctx)
        {
            _factory = dbContextFactory;
            _mapper = mapper;
            _cache = cache;
        }

        public override async Task<GetUserDto?> GetUserAsync(long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var userFromDb = await ctx.Users
                    .AsNoTrackingWithIdentityResolution()
                  .Where(c => c.Id.Equals(userId))
                  .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
                  .FirstOrDefaultAsync();
            return userFromDb;
        }
        public override async Task<User?> GetTrackingUserAsync(long userId)
        {
            var userFromDb = await _ctx.Users
                  .Where(c => c.Id.Equals(userId))
                  .FirstOrDefaultAsync();
            return userFromDb;
        }

        public override async Task<List<long>> GetUserIdsInTeam(long teamId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var userIds = await ctx.Teams
                .Where(t => t.Id.Equals(teamId))
                .Include(t => t.Users)
                .Select(t => t.Users.Select(u => u.Id))
                .FirstOrDefaultAsync();
            if (userIds is null)
                throw new ApplicationException($"Команды с id {teamId} не существует");
            return userIds.ToList();
        }

        public override async Task<List<GetUserDto>> GetUsersFromContainerBySteamId(
            IEnumerable<string> neededSteamIds)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var users = await ctx.Users
                 .Where(u => neededSteamIds.Contains(u.SteamId))
                  .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();
            return users;
        }

        public async override Task CreateUserAsync(User usr)
         => await _ctx.Users.AddAsync(usr);

        public override async Task<GetUserDto?> GetUserBySteamIdAsync(string steamId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var user = await ctx.Users.Where(u => u.SteamId.Equals(steamId))
                .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            return user;
        }

        public async override Task<User> GetTrackingUserBySteamIdAsync(string steamId)
        {
            var userFromDb = await _ctx.Users
                .Where(c => c.SteamId.Equals(steamId))
                .FirstOrDefaultAsync() ??
                throw new ApplicationException(AppDictionary.UserNotExisting);
            return userFromDb;
        }
        private void AddMissingDays(IDictionary<int, decimal> stats, DateTime startOfMonth, DateTime endOfMonth)
        {
            var daysInMonth = (endOfMonth - startOfMonth).Days;
            for (int i = 1; i <= daysInMonth; i++)
            {
                if (!stats.ContainsKey(i))
                    stats[i] = 0;
            }
        }
        private string MonthStatsKey(long userId) => $"user-month-stats-{userId}";
        public async override Task<IDictionary<int, decimal>> Get_User_Profit_Stats_AtLastMonth_GroupedBy_ThreeDay(
            long userId)
        {
            var stats = await _cache.GetObjectAsync<IDictionary<int, decimal>>(MonthStatsKey(userId));
            if (stats is null)
            {
                DateTime nowUtc = DateTime.UtcNow;
                DateTime startOfMonthUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonthUtc = startOfMonthUtc.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                using var ctx = await _factory.CreateDbContextAsync();
                stats = ctx.Lobbies
                    .Where(l => l.CreateTime <= endOfMonthUtc && l.CreateTime >= startOfMonthUtc
                        && l.Status == LobbyStatus.Over)
                    .Select(l => new
                    {
                        Income = Math.Max(0, l.Awards.First(a => a.UserId == userId).Award),
                        Day = (l.CreateTime - startOfMonthUtc).Days
                    })
                    .AsEnumerable()
                    .GroupBy(s => s.Day)
                    .ToDictionary(s => s.Key, s => s.AsEnumerable().Sum(a => a.Income));
                AddMissingDays(stats, startOfMonthUtc, endOfMonthUtc);
                await _cache.SetObjectAsync(MonthStatsKey(userId), stats, GetCacheOptionsForUserStats());
            }
            return stats;
        }
        private async Task<decimal> GetAbsoluteOutcomeFromBalance(long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Pays
                .Where(p => p.UserId.Equals(userId) && p.Description.Equals(AppDictionary.WithdrawalOfFunds))
                .SumAsync(p => p.Amount);
        }
        private string SuperficialKey(long userId) => $"user-superficial-{userId}";
        public async override Task<UserSuperficialStats> GetSuperficialStatsAsync(long userId)
        {
            var stats = await _cache.GetObjectAsync<UserSuperficialStats>(SuperficialKey(userId));
            if (stats is null)
            {
                using var ctx = await _factory.CreateDbContextAsync();

                var balanceTask = ctx.Users.Where(u => u.Id.Equals(userId)).Select(u => u.Balance).FirstAsync();
                var incomeByMonthTask = Get_User_Profit_Stats_AtLastMonth_GroupedBy_ThreeDay(userId);
                var absoluteOutcomeTask = GetAbsoluteOutcomeFromBalance(userId);

                await Task.WhenAll(balanceTask, incomeByMonthTask, absoluteOutcomeTask);

                stats = new UserSuperficialStats()
                {
                    Balance = balanceTask.Result,
                    IncomeByMonth = incomeByMonthTask.Result.Sum(t => t.Value),
                    OutcomeFromBalance = absoluteOutcomeTask.Result
                };
                await _cache.SetObjectAsync(SuperficialKey(userId), stats, GetCacheOptionsForUserStats());
            }

            return stats;
        }
        private string InfographicKey(long userId) => $"user-infographic-{userId}";
        private DistributedCacheEntryOptions GetCacheOptionsForUserStats() => new()
        {
            AbsoluteExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(1),
        };
        public async override Task<UserInfographicStats> GetUserInfographicStatsAsync(long userId)
        {
            var stats = await _cache.GetObjectAsync<UserInfographicStats>(InfographicKey(userId));
            if (stats is null)
            {
                using var ctx = await _factory.CreateDbContextAsync();
                var absoluteReplenishmentsTask = ctx.Pays.Where(p => p.UserId.Equals(userId)
                && p.Description.Equals(AppDictionary.TopUp))
                    .SumAsync(p => p.Amount);
                using var incomeCtx = await _factory.CreateDbContextAsync();
                var absoluteProfitTask = incomeCtx.Lobbies
                    .Where(l => l.Status == LobbyStatus.Over
                       && l.Teams.SelectMany(t => t.Users).Select(u => u.Id).Contains(userId))
                    .Select(l => l.Awards
                        .First(a => a.UserId.Equals(userId)).Award)
                    .SumAsync();
                var absoluteOutcome = GetAbsoluteOutcomeFromBalance(userId);
                await Task.WhenAll(absoluteProfitTask, absoluteOutcome, absoluteReplenishmentsTask);
                var summ = absoluteProfitTask.Result + absoluteOutcome.Result + absoluteReplenishmentsTask.Result +
                    (decimal)10E-20;
                stats = new()
                {
                    Earned = absoluteProfitTask.Result,
                    OutcomeFromBalance = absoluteOutcome.Result,
                    ReplenishedSumm = absoluteReplenishmentsTask.Result,
                    EarnedPercent = (double)(absoluteProfitTask.Result / summ),
                    OutcomeFromBalancePercent = (double)(absoluteOutcome.Result / summ),
                    ReplenishedSummPercent = (double)(absoluteReplenishmentsTask.Result / summ)
                };
                await _cache.SetObjectAsync(InfographicKey(userId), stats, GetCacheOptionsForUserStats());
            }
            return stats;
        }
        private Expression<Func<GetUserView, object?>> GetUserOrderFunction(string orderProperty)
        {
            return orderProperty switch
            {
                "id" => u => u.Id,
                "rate" => u => u.RatePlace,
                "winrate" => u => u.Winrate,
                "matches" => u => u.Matches,
                "lastResults" => u => u.LastResults.Count(l => l.Equals("W")),
                "profit" => u => u.Profit,
                "balance" => u => u.Balance,
                _ => u => u.Name
            };
        }
        public override async Task<List<GetUserView>> GetUsersForAdmin(GetUsersForAdminRequest req)
        {
            long idSearchParam = default(long);
            _ = long.TryParse(req.SearchParam, out idSearchParam);
            using var ctx = await _factory.CreateDbContextAsync();
            var keySelector = GetUserOrderFunction(req.OrderProperty);
            var startUsersQuery = ctx.Users.AsQueryable();
            if (!string.IsNullOrEmpty(req.SearchParam))
            {
                if (idSearchParam != default)
                    startUsersQuery = startUsersQuery.Where(s => s.Id == idSearchParam);
                else
                    startUsersQuery = startUsersQuery.Where(
                        s => s.Name != null && s.Name.ToLower().Contains(
                            req.SearchParam.ToLower()));
            }
            var usersQuery = startUsersQuery
                .ProjectTo<GetUserView>(_mapper.ConfigurationProvider);
            if (req.Order == "asc")
                usersQuery = usersQuery.OrderBy(keySelector);
            else
                usersQuery = usersQuery.OrderByDescending(keySelector);
            usersQuery = usersQuery
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize);
            return await usersQuery
               .ToListAsync();
        }
        private static string RateKey(int page) => $"user-raiting-{page}";
        public override void ClearRaitingCache(int page)
        {
            for (int i = 1; i <= page; i++)
                _cache.Remove(RateKey(i));
        }
        public async override Task<GetUserRateDto[]> GetUsersInRaiting(int page, int? pageSize)
        {
            var ratedUsers = await _cache.GetObjectAsync<GetUserRateDto[]>(RateKey(page));
            if (pageSize is null)
                pageSize = 50;
            if (ratedUsers is null)
            {
                using var ctx = await _factory.CreateDbContextAsync();
                ratedUsers = await ctx.Users
                .Where(u => u.RatePlace != null)
                .OrderBy(u => u.RatePlace)
                .Skip((page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ProjectTo<GetUserRateDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
                await _cache.SetObjectAsync(RateKey(page), ratedUsers, new()
                {
                    AbsoluteExpiration = AppConfig.LastTimeOfRatingUpdate +
                    TimeSpan.FromSeconds(AppConfig.FrequencyOfRaitingUpdating.TotalSeconds)
                });
            }
            return ratedUsers;
        }

        public async override Task<GetUserRateDto[]> GetUsersBatchForRefreshRaiting(int page, int pageSize,
            Expression<Func<User, object>> keySelector)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var usersForRaitingUpdate = await ctx.Users
                .OrderByDescending(keySelector)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetUserRateDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return usersForRaitingUpdate;
        }

        public async override Task<long> GetCurrentLobby(long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var lobbyId = await ctx.Users
                .Where(s => s.Id.Equals(userId))
                .Select(s => s.Teams.Where(t => t.Lobby!.Status == LobbyStatus.Configuring
                || t.Lobby.Status == LobbyStatus.Veto
                || t.Lobby.Status == LobbyStatus.Playing
                || t.Lobby.Status == LobbyStatus.Warmup).Select(t => t.LobbyId)
                .FirstOrDefault())
                .FirstAsync();
            return lobbyId;
        }

        public async override Task<UserArreasInfo> GetUserArreas(long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Users
                .Where(s => s.Id == userId)
                .ProjectTo<UserArreasInfo>(_mapper.ConfigurationProvider)
                .FirstAsync();
        }

        public async override Task<UserAward?> GetUserUnprocessedAward()
        {
            return await _ctx.Awards
                .Where(s => !s.Award.Equals(0) && s.PayTime == DateTime.MinValue)
                .FirstOrDefaultAsync();
        }

        public async override Task<int> GetCountsOfUsersByInterval(string? interval, bool? isOnline)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var query = ctx.Users.AsQueryable();
            if (isOnline != null)
                query = query.Where(u => u.IsOnline == isOnline);
            else
            {
                if (interval != null)
                {
                    var dateInterval = DateIntervals.GetDateInterval(interval);
                    query = query.Where(s => s.LastEnter <= dateInterval.EndDate
                    && s.LastEnter >= dateInterval.StartDate);
                }
            }
            return await query.CountAsync();
        }

        public async override Task<List<long>> GetPlatformAdminsSteamIds()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var admins = await ctx.Users
                .Where(u => u.IsAdmin)
                .Select(u => u.SteamId)
                .ToArrayAsync();
            return admins
                .Select(s => long.Parse(s))
                .ToList();
        }

        public async override Task<List<long>> GetPlatformAdminsUserIds()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var admins = await ctx.Users.Where(u => u.IsAdmin)
                .Select(u => u.Id)
                .ToListAsync();
            return admins;
        }

        public async override Task<bool> UserIsBanned(long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Users
                .Where(s => s.Id == userId)
                .Select(u => u.IsBanned)
                .FirstAsync();
        }

        public async override Task SetUserBanStatus(bool isBanned, long userId)
        {
            await _ctx.Database.ExecuteSqlRawAsync($@"UPDATE public.""Users"" 
            SET ""IsBanned"" = {isBanned}
            WHERE ""Id"" = {userId}");
        }
    }
}
