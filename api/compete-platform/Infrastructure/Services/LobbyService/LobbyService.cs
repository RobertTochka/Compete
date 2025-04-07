using AutoMapper;
using compete_platform.Dto;
using compete_platform.Dto.Common;
using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_poco.Dto;
using compete_poco.Hubs;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using compete_poco.Infrastructure.Services.TimeNotifiers;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models.EventVisitors;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Transactions;

namespace compete_poco.Infrastructure.Services.LobbyService;

public class LobbyService : ILobbyService
{
    private static readonly decimal _minimalBid = 50;
    private readonly CLobbyRepository _lobbySrc;
    private readonly IMapper _mapper;
    private readonly CUserRepository _userSrc;
    private readonly IServerService _serverService;
    private readonly IUserService _userProvider;
    private readonly IDistributedCache _cache;
    private readonly CServerRepository _serverRep;
    private readonly VetoTimeNotifier _vetoNotifier;
    private readonly ILogger<LobbyService> _logger;
    private readonly MatchPrepareNotifier _matchPrepareNotifier;
    private static readonly int availableSecondsToMapSinglePick = 17;
    private static readonly int _minimalAmountOfVeto = 7;
    private static readonly int availableSecondsToConnect = (int)AppConfig.MapInitialWarmupTimeGlobally.TotalSeconds;

    public LobbyService(
        IMapper mapper, 
        IUserService userProvider,
        IDistributedCache cache,
        VetoTimeNotifier vetoNotifier,
        MatchPrepareNotifier matchPrepareNotifier,
        CLobbyRepository lobbySrc,
        CUserRepository userSrc,
        IServerService serverService,
        CServerRepository serverRep,
        ILogger<LobbyService> logger,
        IHubContext<EventHub> hubCtx) 
    {
        _lobbySrc = lobbySrc;
        _mapper = mapper;
        _userSrc = userSrc;
        _serverService = serverService;
        _userProvider = userProvider;
        _cache = cache;
        _serverRep = serverRep;
        _vetoNotifier = vetoNotifier;
        _logger = logger;
        _matchPrepareNotifier = matchPrepareNotifier;
    }
    private async Task LinkUserInLobbyToSteam(GetLobbyDto lobby)
    {
        var linksTasks = new List<IEnumerable<Task<ISteamUserBasedDto<GetUserDto>>>>();
        for (int i = 0; i < lobby.Teams.Count; i++)
        {
            var tasksForLink = lobby.Teams[i].Users
                .Select(u => _userProvider.LinkUserToSteam(u));
            linksTasks.Add(tasksForLink);
        }
        await Task.WhenAll(linksTasks.SelectMany(L => L));
        for (var i = 0; i < linksTasks.Count; i++)
            lobby.Teams[i].Users = linksTasks[i].Select(t => (GetUserDto)t.Result).ToList();
    }
    public async Task<GetLobbyDto> GetLobbyByUserIdAsync(long userId)
    {
        if (!(await _lobbySrc.UserAlreadyInLobby(userId)))
            throw new ApplicationException(AppDictionary.UserNotInLobby);

        var defaultLobby = await _lobbySrc.GetLobbyByUserIdAsync(userId);
        var lobby = _mapper.Map<GetLobbyDto>(defaultLobby);
        await LinkUserInLobbyToSteam(lobby);
        return lobby;
    }
    private async Task AddUserByInvite(JoinToLobbyInfo info, Lobby lobbyToJoin, User joiningUser)
    {
        var labelKey = GetInviteKey((long)info.TeamId!, (long)info.UserId!, (long)info.InviterId!);
        var label = await _cache.GetStringAsync(labelKey);
        if (label is null)
            throw new ApplicationException(AppDictionary.InviteHasExpired);
        var preferredTeam = lobbyToJoin.Teams.First(c => c.Id == info.TeamId);
        if ((int)lobbyToJoin.PlayersAmount == preferredTeam.Users.Count())
            throw new ApplicationException(AppDictionary.TeamAlreadyFull);
        preferredTeam.Users.Add(joiningUser);
    }
    private void DistributeUserForTeam(JoinToLobbyInfo info, Lobby lobbyToJoin, User joiningUser)
    {
        if (!lobbyToJoin.Public)
        {
            if(lobbyToJoin.CodeToConnect != info.Code)
                throw new ApplicationException(AppDictionary.PermissionDenied);
        }
        if (lobbyToJoin.Teams.Count() == 1)
        {
            lobbyToJoin.Teams.Add(new()
            {
                Chat = new(),
                CreatorId = info.UserId,
                Name = $"Team {info.UserId}",
                Users = new List<User>() { joiningUser }
            });
        }
        else
        {
            var preferredTeam = lobbyToJoin.Teams.OrderBy(t => t.Users.Count()).First();
            if (preferredTeam.Users.Count() == (int)lobbyToJoin.PlayersAmount)
                throw new ApplicationException(AppDictionary.LobbyAlreadyFull);
            preferredTeam.Users.Add(joiningUser);
        }
    }
    public async Task<ActionInfo> JoinToLobby(JoinToLobbyInfo info)
    {
        if (!(await CanUserParticipateInLobby(info.UserId)))
            throw new ApplicationException(AppDictionary.CannotParticipateInLobby);

        var lobbyToJoin = await _lobbySrc.GetLobbyForAggregation(info.LobbyId, LobbyStatus.Configuring);
        if (lobbyToJoin is null)
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);

        var joiningUser = await _userSrc.GetTrackingUserAsync(info.UserId) ??
            throw new ApplicationException(AppDictionary.UserNotFound);

        if(info.TeamId != null && info.InviterId != null)
            await AddUserByInvite(info, lobbyToJoin, joiningUser);
        else
            DistributeUserForTeam(info, lobbyToJoin, joiningUser);
        lobbyToJoin.Bids.Add(new() { Bid = 0, UserId = (long)info.UserId! });
        lobbyToJoin.Version = Guid.NewGuid();
        await _lobbySrc.SaveChangesAsync();
        return CreateActionInfo(await GetLobbyByIdAsync(lobbyToJoin.Id));
    }
    private bool RedefineLobbyStructure(Team currentTeam, User leavingUser)
    {
        var shouldDeleteLobby = false;
        if (currentTeam.CreatorId.Equals(leavingUser.Id))
        {
            if (currentTeam.Users.Count() > 1)
            {
                var newCreator = currentTeam.Users
                                                .Where(u => !u.Id
                                                    .Equals(leavingUser.Id)).First().Id;
                currentTeam.CreatorId = newCreator;
                currentTeam.Lobby!.CreatorId = newCreator;
                currentTeam.Users.Remove(leavingUser);
            }
            else
            {

                shouldDeleteLobby = currentTeam.Lobby!.Teams.Count() == 1;
                if (shouldDeleteLobby)
                    //Cascading delete behavior
                    _lobbySrc.Remove(currentTeam.Lobby);
                else
                {
                    var secondTeam = currentTeam.Lobby.Teams.First(t => t.Id != currentTeam.Id);
                    var newCreator = secondTeam.Users
                                                .Where(u => u.Id
                                                    .Equals(secondTeam.CreatorId)).First().Id;
                    currentTeam.Lobby.CreatorId = newCreator;
                    _lobbySrc.Remove(currentTeam);
                }
            }
        }
        else
            currentTeam.Users.Remove(leavingUser);
        return shouldDeleteLobby;
    }

    public async Task<ActionInfo?> LeaveFromLobby(long userId, long lobbyId)
    {
        if (!(await _lobbySrc.UserAlreadyInLobby(userId)))
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        
        var lobby = await _lobbySrc.GetLobbyForAggregation(lobbyId, LobbyStatus.Configuring) ??
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound); 
        var leavingUser = await _userSrc.GetTrackingUserAsync(userId) ??
            throw new ApplicationException(AppDictionary.UserNotFound);
        var currentTeam = lobby.Teams.First(t => t.Users.Select(u => u.Id).Contains(leavingUser.Id));
        var shouldDeleteLobby = RedefineLobbyStructure(currentTeam, leavingUser);
        currentTeam.Lobby!.Version = Guid.NewGuid();
        _lobbySrc.Remove(currentTeam.Lobby!.Bids.First(B => B.UserId.Equals(leavingUser.Id)));
        await _lobbySrc.SaveChangesAsync();
        return shouldDeleteLobby ? null : 
            CreateActionInfo(await GetLobbyByIdAsync(currentTeam.Lobby.Id));
    }
    

    private static void ValidateLobbyOnPlayerAmountChange(Lobby lobby, 
        compete_poco.Models.Type playersAmount)
    {
        if (lobby.Teams.OrderBy(t => t.Users.Count()).Last().Users.Count() > (int)playersAmount)
            throw new ApplicationException(AppDictionary.CannotDefinePlayersAmount);
    }
    public async Task<ActionInfo> SetNewLobbyConfiguration(LobbyAdminConfiguration lobby, long userId)
    {
        var userIsAdmin = await _lobbySrc.UserIsCreatorOfLobby(userId, lobby.Id);
        if (!userIsAdmin)
            throw new ApplicationException(AppDictionary.OnlyCreatorCanEdit);

        var lobbyForUpdate = await _lobbySrc.GetLobbyForAggregation(lobby.Id, LobbyStatus.Configuring) ??
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        if(lobbyForUpdate.PlayersAmount != lobby.PlayersAmount)
            ValidateLobbyOnPlayerAmountChange(lobbyForUpdate, lobby.PlayersAmount);
        lobbyForUpdate.Version = Guid.NewGuid();
        _mapper.Map(lobby, lobbyForUpdate);
        await _lobbySrc.SaveChangesAsync();
        return CreateActionInfo(await GetLobbyByIdAsync(lobbyForUpdate.Id));
    }

    public async Task<ActionInfo> ChangeUserBid(ChangeUserBidRequest request)
    {
        var bid = request.Bid;
        var userId = request.UserId;
        if (bid <= _minimalBid)
            throw new ApplicationException($"Ставка не может быть меньше {_minimalBid}");
        var userBid = await _lobbySrc.GetUserBid(request.UserId) ?? throw new ArgumentException();
        if (userBid.User!.Balance < request.Bid)
            throw new ApplicationException(AppDictionary.NotEnoughMoney);
        userBid.Bid = bid;
        await _lobbySrc.SaveChangesAsync();

        return CreateActionInfo(await GetLobbyByUserIdAsync(userBid.Lobby!.CreatorId));       
    }

    public async Task<ActionInfo> ChangeTeamName(ChangeTeamNameRequest req)
    {
        if (string.IsNullOrEmpty(req.NewName))
            throw new ApplicationException(AppDictionary.TeamNameIsNotValid);
        var team = await _lobbySrc.GetTeamForUpdating(req.UserId);
        if (team is null)
            throw new ApplicationException(AppDictionary.EmptyTeam);
        team.Name = req.NewName;
        await _lobbySrc.SaveChangesAsync();
        return CreateActionInfo(await GetLobbyByIdAsync(team.LobbyId));
    }
    private string GetInviteKey(long teamId, long userId, long inviterId) => $"invite-{inviterId}-{teamId}-{userId}";
    public async Task<JoinToLobbyInfo> CreateInviteForUser(SendInviteRequest req)
    {
        var team = await _lobbySrc.GetTeamForUpdating(req.InviterId) ?? 
            throw new ApplicationException(AppDictionary.NotTeamForInvite);
        var inviteKey = GetInviteKey(team.Id, req.UserId, req.InviterId);
        var options = new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(5) };
        await _cache.SetStringAsync(inviteKey, DateTime.UtcNow.ToString(), options);
        var invite = _mapper.Map<JoinToLobbyInfo>(req);
        invite.TeamId = team.Id;
        invite.LobbyId = team.LobbyId;
        return invite;
    }

    private void ValidateLobbyBid(decimal firstTeamBid, decimal secondTeamBid)
    {
        var max = Math.Max(firstTeamBid, secondTeamBid);
        var min = Math.Min(firstTeamBid, secondTeamBid);
        if (max.Equals(0))
            return;
        if ((max - min) / max * 100 > 10)
            throw new ApplicationException(AppDictionary.TeamBidsBigDifference);
    }
    private void ValidateLobbyParameters(Lobby lobby)
    {
        if (lobby.Teams.Count != 2)
            throw new ApplicationException(AppDictionary.EnemyIsEmpty);
        var currentCapacity = lobby.Teams.OrderBy(t => t.Users.Count).First().Users.Count;
        if (currentCapacity != (int)lobby.PlayersAmount)
            throw new ApplicationException(AppDictionary.TeamsNotFilled);
        var firstTeamUserIds = lobby.Teams.First().Users.Select(u => u.Id);
        var secondTeamUserIds = lobby.Teams.Last().Users.Select(u => u.Id);
        decimal firstTeamBid = 0;
        decimal secondTeamBid = 0;
        foreach(var bid in lobby.Bids)
        {
            if (firstTeamUserIds.Contains(bid.UserId))
                firstTeamBid += bid.Bid;
            else
                secondTeamBid += bid.Bid;
        }
        if (lobby.PickMaps.Count() != _minimalAmountOfVeto)
            throw new ApplicationException(AppDictionary.MapsForVetoDifferentAmount);
        ValidateLobbyBid(firstTeamBid, secondTeamBid);
    }
    public async Task<ActionInfo> StartMapPick(long lobbyId, long userId)
    {
        var lobby = await _lobbySrc.GetLobbyForAggregation(lobbyId, LobbyStatus.Configuring)
            ?? throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        if (lobby.CreatorId != userId)
            throw new ApplicationException(AppDictionary.OnlyCreatorCanEdit);
        ValidateLobbyParameters(lobby);

        lobby.Status = LobbyStatus.Veto;
        lobby.Version = Guid.NewGuid();
        await _lobbySrc.SaveChangesAsync();
        var newLobby = await GetLobbyByIdAsync(lobbyId);
        var actionResponse =  CreateActionInfo(newLobby);
        var rnd = new Random();
        var availableMaps = newLobby.PickMaps
            .Except(newLobby.MapActions
                .Select(a => a.Map))
            .ToList();
        var notifierInfo = new StartNotifierInfo()
        {
            Input = new MapPickRequest()
            {
                UserId = actionResponse.NextPickUserId,
                Map = availableMaps[rnd.Next(availableMaps.Count)],
                LobbyId = lobbyId
            },
            AvailableSeconds = availableSecondsToMapSinglePick,
            LobbyId = lobbyId,
            UserIds = newLobby.Teams.SelectMany(t => t.Users).Select(U => U.Id).ToList()
        };

        _ = _vetoNotifier.StartNotifyAboutTime(notifierInfo);
        return actionResponse;
    }
    private bool GetActionType(List<MapActionInfo> actions, Format matchFormat)
    {
        var currentActionType = !(actions.Count < 2);
        if (currentActionType)
        {
            if (matchFormat.Equals(Format.BO3))
            {
                var lastActions = actions.TakeLast(2);
                var firstPart = lastActions.First().IsPicked;
                var similar = lastActions.All(a => a.IsPicked.Equals(firstPart));
                if (similar)
                    currentActionType = !firstPart;
                else
                    currentActionType = lastActions.Last().IsPicked;
            }
            else if (matchFormat.Equals(Format.BO1))
                currentActionType = false;
        }
        return currentActionType;
    }
    private MapActionInfo DefineLastAction(Lobby lobby)
    {
        var alreadyUsedMaps = lobby.MapActions.Select(a => a.Map);
        var lastMap = lobby.PickMaps.First(m => !alreadyUsedMaps.Contains(m));
        var actionType = true;
        return new() { Map = lastMap, ActionTime = DateTime.UtcNow, IsPicked = actionType, TeamId = lobby.Teams.First().Id };
    }
    private long GetNextPickerId(GetLobbyDto lobby)
    {
        var creatorId = lobby.CreatorId;
        var enemyId = lobby.Teams.FirstOrDefault(t => !t.Users.Select(u => u.Id).Contains(creatorId))?.CreatorId;
        if (enemyId is null)
            enemyId = 0;
        return lobby.MapActions.Count % 2 == 0 ? creatorId : (long)enemyId;   
    }
    private bool MapPickCompeleted(List<MapActionInfo> MapActions, Format format)
    {
        if (MapActions.Count == 0) return false;
        return
        MapActions
        .Where(a => a.IsPicked)
        .Count()
        .Equals(format == Format.BO1 ? 1 : format == Format.BO3 ? 3 : 5);
    }
    private void InitializeMapMatchesInLobby(Lobby needLobby)
    {
        var playingMapsActions = needLobby.MapActions.Where(a => a.IsPicked);
        foreach(var action in playingMapsActions)
        {
            var match = new Match()
            {
                PlayedMap = action.Map,
                Stats = needLobby.Teams.SelectMany(t => t.Users).Select(u => new UserStat() { UserId = u.Id }).ToList()
            };
            needLobby.Matches.Add(match);
        }
    }
    private List<long> GetUserIdsInLobby(Lobby needLobby) => needLobby
        .Teams
        .SelectMany(t => t.Users)
        .Select(U => U.Id)
        .ToList();
    public async Task<ActionInfo> DoAction(MapPickRequest req)
    {
        Lobby? needLobby = null;
        bool isCompleted = false;
        { 
            using var transaction = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                needLobby = await _lobbySrc.GetLobbyForAggregation(req.LobbyId, LobbyStatus.Veto) ??
                    throw new LobbySmoothlyError(AppDictionary.JoinLobbyNotFound);

                if (!needLobby.Teams.Select(t => t.CreatorId).Contains(req.UserId))
                    throw new LobbySmoothlyError(AppDictionary.PermissionDenied);

                if ((needLobby.CreatorId.Equals(req.UserId) && needLobby.MapActions.Count % 2 != 0) ||
                    (!needLobby.CreatorId.Equals(req.UserId) && needLobby.MapActions.Count % 2 != 1))
                    throw new LobbySmoothlyError(AppDictionary.NotYourStep);

                if (!needLobby.PickMaps.Contains(req.Map))
                    throw new LobbySmoothlyError(AppDictionary.DeniedMap);

                var actionType = GetActionType(needLobby.MapActions, needLobby.MatchFormat);
                needLobby.MapActions.Add(new()
                {
                    ActionTime = DateTime.UtcNow,
                    IsPicked = actionType,
                    Map = req.Map,
                    TeamId = needLobby.Teams
                    .First(t => t.Users
                    .Select(u => u.Id)
                    .Contains(req.UserId)).Id
                });
                var onlyOneMapAvailable = needLobby.PickMaps.Except(needLobby.MapActions.Select(a => a.Map)).Count().Equals(1);
                if (onlyOneMapAvailable)
                {
                    var lastAction = DefineLastAction(needLobby);
                    needLobby.MapActions.Add(lastAction);
                }
                isCompleted = MapPickCompeleted(needLobby.MapActions, needLobby.MatchFormat);
                needLobby.Version = Guid.NewGuid();
                if (isCompleted)
                {
                    var userIds = GetUserIdsInLobby(needLobby);
                    var vetoNotifierStopping = _vetoNotifier.StopNotifyingAboutTime(needLobby.Id);
                    needLobby.Status = LobbyStatus.Warmup;
                    needLobby.EdgeConnectTimeOnFirstMap = DateTime.UtcNow + needLobby.WaitToStartTime;
                    needLobby.LastServerUpdate = DateTime.UtcNow;
                    InitializeMapMatchesInLobby(needLobby);
                    await _lobbySrc.SaveChangesAsync();
                    await _serverService.StartServerAsync(needLobby.ServerId, needLobby.Id);
                    _ = vetoNotifierStopping.ContinueWith(t =>
                    {
                        _ = _matchPrepareNotifier.StartNotifyAboutTime(new()
                        {
                            AvailableSeconds = availableSecondsToConnect,
                            LobbyId = needLobby.Id,
                            UserIds = userIds
                        });
                    });
                }
                else
                    await _lobbySrc.SaveChangesAsync();
                transaction.Complete();
            }
            catch
            {
                throw;
            }
        }
        var mapped = await GetLobbyByIdAsync(needLobby.Id);
        var currentVetoState = new ActionInfo()
        {
            IsPickNow = GetActionType(needLobby.MapActions, needLobby.MatchFormat),
            NewLobby = mapped,
            NextPickUserId = GetNextPickerId(mapped),
            PickingComplete = isCompleted
        };
        if (!isCompleted)
        {
            var rnd = new Random();
            var availableMaps = mapped.PickMaps.Except(mapped.MapActions.Select(a => a.Map)).ToList();
            var newInput = new MapPickRequest()
            {
                Map = availableMaps[rnd.Next(availableMaps.Count)],
                UserId = currentVetoState.NextPickUserId,
                LobbyId = needLobby.Id
            };
            _vetoNotifier.RefreshNotifyingAboutTime(needLobby.Id, newInput);
        }
        return currentVetoState;
        
    }

    private ActionInfo CreateActionInfo(GetLobbyDto needLobby) => new ()
    {
        IsPickNow = GetActionType(needLobby.MapActions, needLobby.MatchFormat),
        NewLobby = needLobby,
        NextPickUserId = GetNextPickerId(needLobby),
        PickingComplete = MapPickCompeleted(needLobby.MapActions, needLobby.MatchFormat)
    };
    /// <summary>
    /// Является оберткой над репозиторием лобби для получения лобби из бд, 
    /// дополнительно линкует пользователей лобби к стиму
    /// </summary>
    /// <param name="lobbyId">Id для поиска</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    private async Task<GetLobbyDto> GetLobbyByIdAsync(long lobbyId)
    {
        var defaultLobby = await _lobbySrc.GetLobbyByIdAsync(lobbyId) ??
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        var lobby = _mapper.Map<GetLobbyDto>(defaultLobby);
        await LinkUserInLobbyToSteam(lobby);
        return lobby;
    }
   
    public async Task<ActionInfo> GetLobbyVetoStateAsyncById(long lobbyId, long userId)
    {
        var needLobbyTask = GetLobbyByIdAsync(lobbyId);
        var adminsTask = _userSrc.GetPlatformAdminsUserIds();
        await Task.WhenAll(adminsTask, needLobbyTask);
        var needLobby = needLobbyTask.Result;
        var admins = adminsTask.Result;
        if (!needLobby.Public && 
            !needLobby.Teams.SelectMany(t => t.Users.Select(u => u.Id)).Contains(userId)
            && !admins.Contains(userId))
            throw new ApplicationException(AppDictionary.PermissionDenied);
        return CreateActionInfo(needLobby);
    }

    public async Task<ActionInfo> GetLobbyVetoStateAsyncByUserId(long userId)
    {
        var needLobbyTask = GetLobbyByUserIdAsync(userId);
        var adminsTask = _userSrc.GetPlatformAdminsUserIds();
        await Task.WhenAll(needLobbyTask, adminsTask);
        var needLobby = needLobbyTask.Result;
        var admins = adminsTask.Result;
        if (!needLobby.Public 
            && !needLobby.Teams.SelectMany(t => t.Users.Select(u => u.Id)).Contains(userId)
            && !admins.Contains(userId))
            throw new ApplicationException(AppDictionary.PermissionDenied);
        return CreateActionInfo(needLobby);
    }

    public async Task<List<GetLobbyViewDto>> GetLobbiesAsync(GetLobbyRequest request)
    {
        var lobbies = await _lobbySrc.GetLobbiesAsync(request);
        await _userProvider.LinkUsersToSteam(lobbies.Select(l => (ISteamUserBasedDto<GetUserDto>)l.Creator).ToList(), false);
        return lobbies;
    }
    public async Task<List<GetLobbyViewDto>> GetAllLobbiesAsync()
    {
        var lobbies = await _lobbySrc.GetAllLobbiesAsync();
        await _userProvider.LinkUsersToSteam(lobbies.Select(l => (ISteamUserBasedDto<GetUserDto>)l.Creator).ToList(), false);
        return lobbies;
    }
    private async Task<bool> CanUserParticipateInLobby(long userId)
    {
        var userAlreadyInLobbyTask = _lobbySrc.UserAlreadyInLobby(userId);
        var userIsBannedTask = _userSrc.UserIsBanned(userId);
        await Task.WhenAll(userAlreadyInLobbyTask, userIsBannedTask);
        var result =  !userAlreadyInLobbyTask.Result && !userIsBannedTask.Result;
        return result;
    }
    public async Task<GetLobbyDto> CreateLobbyAsync(long creatorId)
    {
        if(!(await CanUserParticipateInLobby(creatorId)))
            throw new ApplicationException(AppDictionary.CannotParticipateInLobby);

        var serversAvailableTask = _serverRep.GetAvailableServersAsync();
        Lobby lobby = new()
        {
            Public = false,
            CreatorId = creatorId,
            Chat = new(),
            Bids = new List<UserBid>() { new() { Bid = 0, UserId = creatorId } },
            WaitToStartTime = AppConfig.MapInitialWarmupTimeGlobally,
            CodeToConnect = Guid.NewGuid(),
        };
        var creator = await _userSrc.GetTrackingUserAsync(creatorId);
      
        if (creator is null)
            throw new ApplicationException($"Пользователя с {creatorId} не существует");
        var creatorTeam = new Team()
        {
            Chat = new(),
            CreatorId = creatorId,
            Name = $"Team {creator.Id}"
        };
        creator.Teams.Add(creatorTeam);
        lobby.Teams.Add(creatorTeam);
        lobby.CreateTime = DateTime.UtcNow;
        await serversAvailableTask;
        var defaultServer = serversAvailableTask.Result.FirstOrDefault();
        if (defaultServer is null)
            throw new ApplicationException(AppDictionary.ServersAreNotAvailable);
        else
            lobby.ServerId = defaultServer.Id;
        await _lobbySrc.CreateLobbyAsync(lobby);
        await _lobbySrc.SaveChangesAsync();
        return _mapper.Map<GetLobbyDto>(lobby);
    }
    private static void SetupLobbyOnCancel(Lobby lobby)
    {
        lobby.LastServerUpdate = DateTime.UtcNow;
        lobby.Version = Guid.NewGuid();
        lobby.EventVisitor = new LobbyEndedEvent();
        lobby.Status = LobbyStatus.Canceled;
    }
    private async Task CancelCompletedLobby(Lobby lobby, long[]? offenders = null)
    {
        SetupLobbyOnCancel(lobby);
        var antiAwards = lobby.Awards.Select(a => new UserAward()
        {
            Award = a.Award * -1,
            AwardType = AwardType.MatchCanceled,
            UserId = a.UserId,
            PayTime = DateTime.MinValue
        }).ToList();
        antiAwards.AddRange(lobby.Awards);
        lobby.Awards = antiAwards;
        await _lobbySrc.SaveChangesAsync();
    }
    private  async Task CancelActiveLobby(Lobby lobby, long[]? offenders = null)
    {
        SetupLobbyOnCancel(lobby);
        try
        {
            await _matchPrepareNotifier.StopNotifyingAboutTime(lobby.Id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Лобби {lobby.Id} было отменено, но таймер подготовки остуствовал." +
                $" Это говорит о том, что на сервере не было игроков\n" +
                $"{ex.Message}");
        }
        var awards = lobby.Bids
            .Where(s => (!offenders?.Contains(s.UserId)) ?? true)
            .Select(b => new UserAward()
            {
                AwardType = AwardType.MatchCanceled,
                Award = 0,
                UserId = b.UserId,
                PayTime = DateTime.UtcNow
            }).ToList();
        awards.AddRange(lobby.Bids
            .Where(s => offenders?.Contains(s.UserId) ?? false)
            .Select(b => new UserAward()
            {
                AwardType = AwardType.Lose,
                Award = b.Bid * -1,
                UserId = b.UserId,
                PayTime = DateTime.MinValue
            }));
        lobby.Awards = awards;
        await _lobbySrc.SaveChangesAsync();
        await _serverService.StopServerAsync(lobby.ServerId, lobby.Id);
    }
    public async Task<ActionInfo> Cancel_Lobby(long lobbyId, long[]? offenders = null)
    {
        using (var transaction = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled))
        {
            var lobby = await _lobbySrc.GetLobbyForAggregation(
                lobbyId, LobbyStatus.Warmup, LobbyStatus.Playing, LobbyStatus.Over) ??
                throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
            if (lobby.Status != LobbyStatus.Over)
            {
                await CancelActiveLobby(lobby, offenders);
            }
            else
            {
                await CancelCompletedLobby(lobby, offenders);
            }
            transaction.Complete();
        }
        var action = CreateActionInfo(await GetLobbyByIdAsync(lobbyId));
        return action;
    }
    private static LobbyResult GetLobbyResult(Lobby lobby)
    {
        Dictionary<long, int> winCounter = new();
        foreach (var match in lobby.Matches)
        {
            if (match.TeamId == null)
                continue;
            var teamWinnerId = (long)match.TeamId;
            if (!winCounter.ContainsKey(teamWinnerId))
                winCounter[teamWinnerId] = 0;
            winCounter[teamWinnerId]++;
        }
        var teamWinner = winCounter.MaxBy(p => p.Value).Key;
        var teamLoser = lobby.Teams.First(t => !t.Id.Equals(teamWinner)).Id;
        var teamWinnerUserIds = lobby.Teams
            .First(t => t.Id.Equals(teamWinner)).Users.Select(U => U.Id)
            .ToArray();
        var teamLoserUserIds = lobby.Teams
            .First(t => t.Id.Equals(teamLoser)).Users.Select(u => u.Id)
            .ToArray();
        var userWinnerBids = lobby.Bids.Where(b => teamWinnerUserIds.Contains(b.UserId)).ToArray();
        var teamWinnerFund = userWinnerBids.Sum(b => b.Bid);
        if (teamWinnerFund.Equals(0))
            teamWinnerFund += 1;
        var lobbyFund = lobby.Bids.Sum(b => b.Bid);
        if (lobbyFund.Equals(0))
            lobbyFund += 1;
        var userWinnerIncomePercent = userWinnerBids
            .ToDictionary(b => b.UserId, b => b.Bid / teamWinnerFund);
        return new LobbyResult(
            teamWinner, teamLoser, teamWinnerUserIds,
            teamLoserUserIds, userWinnerBids, teamWinnerFund, lobbyFund, userWinnerIncomePercent);
    }
    private static void SetupLobbyOnEnd(Lobby lobby)
    {
        lobby.Status = LobbyStatus.Over;
        lobby.EventVisitor = new LobbyEndedEvent();
        var lobbyResult = GetLobbyResult(lobby);
        var awards = lobbyResult.UserWinnerIncomePercent.Select(p => new UserAward() 
        { 
            UserId = p.Key,
            Award = (
            (lobbyResult.LobbyFund * 
            (1 - AppConfig.AmountOfComission)) - lobbyResult.TeamWinnerFund) * p.Value,
            AwardType = AwardType.Win,
            PayTime = DateTime.MinValue
        }).ToList();
        awards.AddRange(lobbyResult.TeamLoserUserIds.Select(u => new UserAward()
        {
            UserId = u,
            Award = lobby.Bids.First(b => b.UserId == u).Bid * -1,
            AwardType = AwardType.Lose,
            PayTime = DateTime.MinValue
        }).ToList());
        lobby.Awards = awards;
        lobby.TeamWinner = lobbyResult.TeamWinner;
    }
    private bool WasLastMap(Lobby lobby)
    {
        var mapWins = new Dictionary<long, int>();
        foreach (var team in lobby.Teams)
        {
            foreach(var match in lobby.Matches)
            {
                if(match.TeamId == team.Id)
                {
                    if(!mapWins.Keys.Contains(team.Id))
                        mapWins[team.Id] = 0;
                    mapWins[team.Id]++;
                }
            }
        }
        var mostWinsTeamValue = mapWins.MaxBy(p => p.Value).Value;
        if (mostWinsTeamValue.Equals(2) && lobby.MatchFormat == Format.BO3)
            return true;
        if(mostWinsTeamValue.Equals(1) && lobby.MatchFormat == Format.BO1)
            return true;
        if (mostWinsTeamValue.Equals(3) && lobby.MatchFormat == Format.BO5)
            return true;
        return lobby.Matches.Last().TeamId != null;
    }
    public async Task<MapEndInformation> LobbyMapEnded(long lobbyId)
    {
        var lobby = await _lobbySrc.GetLobbyForAggregation(lobbyId, LobbyStatus.Playing) ??
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        var mapEndedInformation = new MapEndInformation()
        {
            EndOfWarmup = DateTime.UtcNow + AppConfig.MapInitialWarmupTimeGlobally
        };
        lobby.LastServerUpdate = DateTime.UtcNow;
        lobby.Version = Guid.NewGuid();
        if (WasLastMap(lobby))
        {
            using var transaction = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
            SetupLobbyOnEnd(lobby);
            await _lobbySrc.SaveChangesAsync();
            await _serverService.StopServerAsync(lobby.ServerId, lobby.Id);
            transaction.Complete();
        }
        else
        {
            lobby.Status = LobbyStatus.Warmup;
            var userIds = GetUserIdsInLobby(lobby);
            _ = _matchPrepareNotifier.StartNotifyAboutTime(new()
            {
                AvailableSeconds = (int)AppConfig.MapInitialWarmupTimeGlobally.TotalSeconds,
                LobbyId = lobby.Id,
                UserIds = userIds
            });
            await _lobbySrc.SaveChangesAsync();
        }
        var finalLobby = await GetLobbyByIdAsync(lobbyId);
        mapEndedInformation.NewLobby = CreateActionInfo(finalLobby);
       
        return mapEndedInformation;
    }

    public async Task AllConnectedConfirmation(long lobbyId)
    {
        var lobby = await _lobbySrc.GetLobbyForAggregation(lobbyId, LobbyStatus.Warmup) ??
            throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        lobby.Status = LobbyStatus.Playing;
        await _matchPrepareNotifier.StopNotifyingAboutTime(lobbyId);
        lobby.LastServerUpdate = DateTime.UtcNow;
        lobby.Version = Guid.NewGuid();
        await _lobbySrc.SaveChangesAsync();
    }

    public async Task<ActionInfo> ResetLobbyAfterFailedVeto(long lobbyId)
    {
        var lobby =  await _lobbySrc.GetLobbyForAggregation(lobbyId, LobbyStatus.Veto) ??
                    throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
        int retryCount = 0;

        while (retryCount < AppDictionary.MaxRetryCount)
        {
            try
            {
                lobby.MapActions = new();
                lobby.Status = LobbyStatus.Configuring;
                lobby.Version = Guid.NewGuid();
                lobby.EventVisitor = new LobbyFailedEvent();
                await _lobbySrc.SaveChangesAsync();
                break;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;

                foreach (var entry in ex.Entries)
                {
                    var databaseEntry = await entry.GetDatabaseValuesAsync();
                    if (databaseEntry is null)
                        throw new InvalidOperationException(AppDictionary.NotExistingAlready);

                    entry.OriginalValues.SetValues(databaseEntry);

                }

                if (retryCount >= AppDictionary.MaxRetryCount)
                {
                    throw new Exception(AppDictionary.RetryExceeded);
                }
            }
        }
        var mapped = await GetLobbyByIdAsync(lobbyId);
        return CreateActionInfo(mapped);
    }
}
