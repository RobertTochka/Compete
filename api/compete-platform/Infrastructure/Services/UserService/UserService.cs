using AutoMapper;
using compete_platform;
using compete_platform.Dto;
using compete_platform.Dto.Common;
using compete_platform.Infrastructure.Services.PayEventer;
using compete_poco.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models.EventVisitors;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Infrastructure.PropertyResolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System.Data;
using System.Linq.Expressions;
namespace compete_poco.Infrastructure.Services.UserService;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly SteamWebInterfaceFactory _steamFactory;
    private readonly HttpClient _client;
    private readonly ILogger<UserService> _logger;
    private readonly AppConfig _cfg;
    private readonly CUserRepository _userSrc;
    private readonly IDistributedCache _cache;
    private readonly IPayEventer _payEventer;

    public UserService(CUserRepository userSrc,
        IMapper mapper,
        ILogger<UserService> logger,
        SteamWebInterfaceFactory steamFactory,
        HttpClient client,
        IDistributedCache cache,
        AppConfig cfg,
        IPayEventer payEventer)
    {
        _mapper = mapper;
        _steamFactory = steamFactory;
        _client = client;
        _logger = logger;
        _cfg = cfg;
        _userSrc = userSrc;
        _cache = cache;
        _payEventer = payEventer;
    }
    private bool IsTimeToUpdateSteamCredentials(GetUserDto userDto) =>
         userDto.LastSteamInfoUpdate + AppConfig.FrequencyOfSteamUserRefreshing < DateTime.UtcNow
        || userDto.Name.Equals(string.Empty) || userDto.AvatarUrl.Equals(string.Empty);
    private async Task UpdateSteamCredentials(GetUserDto userDto)
    {
        var userForUpdate = await _userSrc.GetTrackingUserAsync(userDto.Id) ??
                throw new InvalidProgramException("Пользователь был удален");
        userForUpdate.Name = userDto.Name;
        userForUpdate.AvatarUrl = userDto.AvatarUrl;
        userForUpdate.LastSteamInfoUpdate = DateTime.UtcNow;
        await _userSrc.SaveChangesAsync();
    }
    public async Task<GetUserDto> GetUserAsync(GetUserRequest req)
    {
        var userFromDb = await _userSrc.GetUserAsync(req.UserId);
        if (userFromDb is null)
            throw new ApplicationException(AppDictionary.UserNotExisting);
        userFromDb = (GetUserDto)(await LinkUserToSteam(userFromDb, req.IncludeFriends));
        if (IsTimeToUpdateSteamCredentials(userFromDb))
            await UpdateSteamCredentials(userFromDb);
        return userFromDb;
    }
    private async Task<IEnumerable<ulong>> GetSteamFriends(ulong steamId)
    {
        var cacheKey = $"steam-friends-{steamId}";
        var friendsModel = await _cache.GetObjectAsync<IReadOnlyCollection<FriendModel>>(cacheKey);
        if (friendsModel == null)
        {
            var steamInterface = _steamFactory.CreateSteamWebInterface<SteamUser>(_client);
            try
            {
                friendsModel = (await steamInterface.GetFriendsListAsync(steamId)).Data;
                await _cache.SetObjectAsync(cacheKey, friendsModel);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation($"Пользователь {steamId} запретил просматривать список друзей");
                    friendsModel = Enumerable.Range(0, 0).Select(u => new FriendModel()).ToList();
                    await _cache.SetObjectAsync(cacheKey, friendsModel);
                    return friendsModel.Select(t => t.SteamId);
                }
                else
                    throw new Exception(ex.Message, ex);
            }
        }
        return friendsModel.Select(f => f.SteamId);
    }
    private async Task LinkFriendsToUsers(List<ISteamUserBasedDto<GetUserDto>> users)
    {
        var userSteamFriendsIdsTasks = new List<Task<IEnumerable<ulong>>>();
        foreach (var user in users)
            userSteamFriendsIdsTasks.Add(GetSteamFriends(ulong.Parse(user.SteamId)));
        await Task.WhenAll(userSteamFriendsIdsTasks);
        var allSteamIds = userSteamFriendsIdsTasks.Select(t => t.Result)
            .SelectMany(t => t)
            .Select(u => u.ToString());
        var mapResult = new Dictionary<int, List<GetUserDto>>(users.Count);
        var neededUsers = (await _userSrc
            .GetUsersFromContainerBySteamId(allSteamIds));

        for (int i = 0; i < userSteamFriendsIdsTasks.Count; i++)
        {
            var userSteamFriendsTask = userSteamFriendsIdsTasks[i];
            foreach (var user in neededUsers)
            {
                if (userSteamFriendsTask.Result.Contains(ulong.Parse(user.SteamId)))
                {
                    if (!mapResult.ContainsKey(i))
                        mapResult[i] = new();
                    mapResult[i].Add(user);
                }
            }
        }
        var steamLinking = new List<Task>();
        for (int i = 0; i < users.Count; i++)
        {
            if (mapResult.ContainsKey(i))
                users[i].Friends = mapResult[i];
        }
        await Task.WhenAll(steamLinking);
    }
    public async Task<List<ISteamUserBasedDto<GetUserDto>>> LinkUsersToSteam(List<ISteamUserBasedDto<GetUserDto>>
        mappedUsers, bool includeFriends = false)
    {
        IEnumerable<PlayerSummaryModel>? userSummaries;
        var userSummariesFromCache = mappedUsers
            .Select(u => _cache.GetObjectAsync<PlayerSummaryModel>($"steam-summary-{u.SteamId}"));
        await Task.WhenAll(userSummariesFromCache);
        if (userSummariesFromCache.Any(t => t.Result is null))
        {
            var steamInterface = _steamFactory.CreateSteamWebInterface<SteamUser>(_client);
            var summariesResponse = await steamInterface.GetPlayerSummariesAsync(mappedUsers
                .Select(u => ulong.Parse(u.SteamId))
                    .ToList());
            userSummaries = summariesResponse.Data.AsEnumerable();
        }
        else
            userSummaries = userSummariesFromCache.Select(t => t.Result!);
        var result = new List<ISteamUserBasedDto<GetUserDto>>();
        foreach (var corrected in userSummaries
            .Join(mappedUsers, s => s.SteamId, u => ulong.Parse(u.SteamId), (s, u) => (s, u)))
        {
            var user = corrected.u;
            var steamUser = corrected.s;
            user.Name = steamUser.Nickname;
            user.AvatarUrl = steamUser.AvatarFullUrl;
            result.Add(user);
        }
        if (includeFriends)
            await LinkFriendsToUsers(result);
        return result;
    }

    public async Task<ISteamUserBasedDto<GetUserDto>> LinkUserToSteam(ISteamUserBasedDto<GetUserDto> user,
        bool includeFriends = false)
    {
        var userCacheKey = $"steam-summary-{user.SteamId}";
        var userSummary = await _cache.GetObjectAsync<PlayerSummaryModel>(userCacheKey);
        if (userSummary == null)
        {
            var steamInterface = _steamFactory.CreateSteamWebInterface<SteamUser>(_client);
            var userSummaryResponse = await steamInterface.GetPlayerSummaryAsync(ulong.Parse(user.SteamId));
            userSummary = userSummaryResponse.Data;
            await _cache.SetObjectAsync(userCacheKey, userSummary);
        }
        user.AvatarUrl = userSummary.AvatarFullUrl;
        user.Name = userSummary.Nickname;
        if (includeFriends)
        {
            var list = new List<ISteamUserBasedDto<GetUserDto>>() { user };
            await LinkFriendsToUsers(list);
            return list.First();
        }
        return user;
    }

    public async Task SetUserAvailability(long userId, bool isOnline)
    {
        using (var transaction = await _userSrc.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            var user = await _userSrc.GetTrackingUserAsync(userId) ??
                throw new ApplicationException(AppDictionary.UserNotExisting);
            user.IsOnline = isOnline;
            if (isOnline)
                user.LastEnter = DateTime.UtcNow;
            await _userSrc.SaveChangesAsync();
            transaction.Commit();
        }
    }

    public async Task<User> CreateUser(UserRole role, string steamId)
    {
        var newUser = new User();
        newUser.SteamId = steamId;
        newUser.RegistrationDate = DateTime.UtcNow;
        newUser.Role = UserRole.User;
        newUser.EventVisitor = new UserCreatedEvent();
        await _userSrc.CreateUserAsync(newUser);
        await _userSrc.SaveChangesAsync();
        return newUser;
    }

    public async Task<bool> UserIsExists(long userId)
    {
        var user = await _userSrc.GetUserAsync(userId);
        return user != null;
    }
    private async Task UpdateUserRating(GetUserRateDto ratedUser)
    {
        var rating = UserStatResolvers.CalculatePlayerRating(ratedUser.Winrate, ratedUser.KillsByDeaths,
                   ratedUser.Income, ratedUser.HeadshotPercent);
        var user = (await _userSrc.GetTrackingUserAsync(ratedUser.Id))!;
        user.Rate = rating;
        await _userSrc.SaveChangesAsync();
    }
    private async Task UpdateRatePlaceForSingleUser(GetUserRateDto ratedUser, long place)
    {
        var userId = ratedUser.Id;
        var user = (await _userSrc.GetTrackingUserAsync(userId))!;

        user.RatePlace = place;
        await _userSrc.SaveChangesAsync();
    }

    public async Task UpdateUsersRaiting()
    {
        Expression<Func<User, object>> rateKeySelector = u => u.RegistrationDate;
        int butchSize = 100;
        int page = 1;
        while (true)
        {
            var ratedUsers = await _userSrc.GetUsersBatchForRefreshRaiting(page, butchSize, rateKeySelector);
            page++;
            foreach (var ratedUser in ratedUsers)
                await UpdateUserRating(ratedUser);

            if (ratedUsers.Length < butchSize)
                break;
        }
        page = 1;
        _userSrc.ClearCache();
        long usersCounter = 0;
        Expression<Func<User, object>> ratePlaceKeySelector = u => u.Rate;
        while (true)
        {
            var ratedUsers = await _userSrc.GetUsersBatchForRefreshRaiting(page, butchSize, ratePlaceKeySelector);
            page++;
            foreach (var ratedUser in ratedUsers)
            {
                usersCounter++;
                await UpdateRatePlaceForSingleUser(ratedUser, usersCounter);
            }
            if (ratedUsers.Length < butchSize)
                break;
        }
        _userSrc.ClearRaitingCache(page);
    }

    public async Task TopUpUserBalance(decimal amount, long userId, string? payId = null,
        string? payCorellation = null,
        string? reason = null, bool shouldUseTransaction = true)
    {
        if (shouldUseTransaction)
        {
            using var transaction = (await _userSrc.BeginTransaction(IsolationLevel.RepeatableRead));
            _logger.LogInformation($"Вызван метод для пополнения баланса для пользователя {userId}");
            var user = await _userSrc.GetTrackingUserAsync(userId) ??
            throw new ApplicationException(AppDictionary.UserNotFound);

            _logger.LogInformation($"Изначальный баланс пользователя {userId} - [{user.Balance}]");
            user.Balance += amount;
            await _userSrc.SaveChangesAsync();
            if (payId != null && reason == null && payCorellation != null)
                await _payEventer.PaySuccessEvent(userId, amount, payId, payCorellation);
            if (payId != null && reason != null && payCorellation != null)
                await _payEventer.PayoutFailedEvent(userId, amount,
                    payId, reason, payCorellation);
            transaction.Commit();
            _logger.LogInformation($"Конечный баланс пользователя {userId} это {user.Balance}, " +
            $"счет был пополнен на {amount}");
        }
        else
        {
            _logger.LogInformation($"Вызван метод для пополнения баланса для пользователя {userId}");
            var user = await _userSrc.GetTrackingUserAsync(userId) ??
            throw new ApplicationException(AppDictionary.UserNotFound);
            _logger.LogInformation($"Изначальный баланс пользователя {userId} - [{user.Balance}]");
            user.Balance += amount;
            await _userSrc.SaveChangesAsync();
            if (payId != null && reason == null && payCorellation != null)
                await _payEventer.PaySuccessEvent(userId, amount, payId, payCorellation);
            if (payId != null && reason != null && payCorellation != null)
                await _payEventer.PayoutFailedEvent(userId, amount,
                    payId, reason, payCorellation);
            _logger.LogInformation($"Конечный баланс пользователя {userId} это {user.Balance}, " +
            $"счет был пополнен на {amount}");
        }
    }

    public async Task<string> WithdrawalFunds(long userId, decimal amount)
    {
        using var transaction = (await _userSrc.BeginTransaction(IsolationLevel.RepeatableRead));
        var userAreas = await _userSrc.GetUserArreas(userId);

        var finalLose = userAreas.ArreasByAvailableBids - userAreas.ArreasByLoseAward; // последнее число отрицательное, поэтому -
        var user = await _userSrc.GetTrackingUserAsync(userId) ??
        throw new ApplicationException(AppDictionary.UserNotExisting);
        _logger.LogInformation($"Пришел запрос на снятие средств пользователем {userId}");
        _logger.LogInformation($"Начальный баланс пользователя {userId} - [{user.Balance}]");
        if (user.Balance - finalLose < amount)
            throw new ApplicationException(AppDictionary.NotEnoughMoney);
        user.Balance -= amount;
        var correlationId = await _payEventer.PayoutRequestedEvent(userId, amount);
        await _userSrc.SaveChangesAsync();
        transaction.Commit();
        return correlationId;
    }

    public async Task<compete_platform.Dto.UserStatus> GetUserStatusForLobby(long userId)
    {
        var lobbyId = await _userSrc.GetCurrentLobby(userId);
        return new() { LobbyId = lobbyId, Active = lobbyId != default };
    }

    public async Task<bool> HandleUnproccesedUserAward()
    {
        using (var trsansaction = await _userSrc.BeginTransaction(IsolationLevel.RepeatableRead))
        {
            var award = await _userSrc.GetUserUnprocessedAward();
            if (award is null)
            {
                trsansaction.Rollback();
                return false;
            }
            await TopUpUserBalance(award.Award, award.UserId, null, null, null, false);
            award.PayTime = DateTime.UtcNow;
            await _userSrc.SaveChangesAsync();
            trsansaction.Commit();
            if (award.User != null && award.User.Balance < 0)
                _logger.LogError($"Нарушена целостность баланса для {award.UserId}");
            return true;
        }
    }

    public async Task SetUserBanStatus(bool isBanned, long userId, long sourceId)
    {
        if (!(await _userSrc.GetPlatformAdminsUserIds()).Contains(sourceId))
            throw new ApplicationException(AppDictionary.PermissionDenied);
        await _userSrc.SetUserBanStatus(isBanned, userId);
    }
}
