using AutoMapper;
using AutoMapper.QueryableExtensions;
using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_platform.Infrastructure.ValueResolvers;
using compete_poco.Dto;
using compete_poco.Infrastructure.Data;
using compete_poco.Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Services
{
    public class LobbyRepository : CLobbyRepository
    {
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<ApplicationContext> _factory;

        public LobbyRepository(ApplicationContext ctx,
            IMapper mapper,
            IDbContextFactory<ApplicationContext> factory) : base(ctx)
        {
            _mapper = mapper;
            _factory = factory;
        }

        public override async Task CreateLobbyAsync(Lobby newLobby)
        {
            await _ctx.Lobbies.AddAsync(newLobby);
        }

        private List<GetLobbyViewDto> FilterLobbies(List<GetLobbyViewDto> lobbies, GetLobbyRequest request)
        {
            if (!string.IsNullOrEmpty(request.Status))
            {
                var status = (LobbyStatus) Enum.Parse(typeof(LobbyStatus), request.Status);
                lobbies = lobbies.Where(l => l.Status.Equals(status)).ToList();
            }
            if (!string.IsNullOrEmpty(request.Type))
            {
                var type = (Models.Type) Enum.Parse(typeof(Models.Type), request.Type);
                lobbies = lobbies.Where(l => l.PlayersAmount.Equals(type)).ToList();
            }
            if (!string.IsNullOrEmpty(request.Mode))
            {
                var mode = (Format) Enum.Parse(typeof(Format), request.Mode);
                lobbies = lobbies.Where(l => l.MatchFormat.Equals(mode)).ToList();
            }
            if (request.Maps != null && request.Maps.Any())
            {
                var maps = request.Maps.Split('-').Select(m => (Map) Enum.Parse(typeof(Map), m)).ToList();
                lobbies = lobbies.Where(l => l.PickMaps != null && maps.Any(m => l.PickMaps.Contains(m))).ToList();
            }
            if (!string.IsNullOrEmpty(request.Nickname))
            {
                lobbies = lobbies.Where(l => l.Creator.Name.ToLower().Contains(request.Nickname.ToLower())).ToList();
            }

            return lobbies;
        }

        public override async Task<List<GetLobbyViewDto>> GetLobbiesAsync(GetLobbyRequest request)
        {
            var result = await _ctx.Lobbies
                .Include(l => l.Creator)
                .Include(l => l.Bids)
                .Where(l => !l.Status.Equals(LobbyStatus.Over) && !l.Status.Equals(LobbyStatus.Canceled) && l.Public)
                .OrderByDescending(l => l.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<GetLobbyViewDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            result = FilterLobbies(result, request);

            return result;
        }
        public override async Task<List<GetLobbyViewDto>> GetAllLobbiesAsync()
        {
            var result = await _ctx.Lobbies
                .Include(l => l.Creator)
                .Include(l => l.Bids)
                .Where(l => l.Status.Equals(LobbyStatus.Configuring) && l.Public)
                .OrderByDescending(l => l.Id)
                .ProjectTo<GetLobbyViewDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return result;
        }

        public override async Task<Lobby> GetLobbyByIdAsync(long id)
        {
            var lobbyFromDb = await _ctx.Lobbies
               .AsNoTracking()
               .Include(l => l.Teams.OrderBy(t => t.Id))
                   .ThenInclude(t => t.Users.OrderBy(t => t.Id))
               .Include(l => l.Awards)
               .Include(l => l.Bids)
               .Include(l => l.Matches)
               .Include(l => l.Server)
               .FirstAsync(t => t.Id.Equals(id));

            return lobbyFromDb;
        }

        public override async Task<Lobby> GetLobbyByUserIdAsync(long userId)
        {
            var userWithLobby = await _ctx.Users
               .AsNoTrackingWithIdentityResolution()
               .Where(u => u.Id.Equals(userId))
               .Include(u => u.Teams.Where(t => t.Lobby!.Status.Equals(LobbyStatus.Veto) ||
                    t.Lobby!.Status.Equals(LobbyStatus.Configuring) ||
                    t.Lobby!.Status.Equals(LobbyStatus.Playing) ||
                    t.Lobby!.Status.Equals(LobbyStatus.Warmup))
               .OrderBy(t => t.Id))
               .ThenInclude(t => t.Lobby)
               .ThenInclude(l => l!.Teams)
               .ThenInclude(t => t.Users)
               .OrderBy(t => t.Id)
               .Include(l => l.Teams)
               .ThenInclude(t => t.Lobby)
               .ThenInclude(l => l!.Bids)
               .Include(u => u.Teams)
               .ThenInclude(t => t.Lobby)
               .ThenInclude(l => l!.Server)
               .Include(u => u.Teams)
               .ThenInclude(t => t.Lobby)
               .ThenInclude(l => l!.Matches)
               .FirstAsync();
            var lobby = userWithLobby.Teams.FirstOrDefault()?.Lobby ??
                throw new ApplicationException(AppDictionary.JoinLobbyNotFound);
            return lobby;
        }

        public async override Task<int> GetLobbyCountForPlayerAmount(Models.Type playersAmount)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var lobbyCount = await ctx.Lobbies
                .Where(l => l.PlayersAmount == playersAmount && l.Status == LobbyStatus.Configuring)
                .CountAsync();
            return lobbyCount;
        }

        public override async Task<Lobby?> GetLobbyForAggregation(long lobbyId, params LobbyStatus[] statuses)
        {
            var lobby = await _ctx.Lobbies
                .Include(l => l.Matches)
                .Include(l => l.Teams.OrderBy(t => t.Id))
                .ThenInclude(t => t.Users)
                .Include(l => l.Awards)
                .Include(l => l.Bids)
                .FirstOrDefaultAsync(l => l.Id.Equals(lobbyId) &&
                    statuses.Contains(l.Status));

            return lobby;
        }

        public async override Task<Lobby> GetLobbyForLogInformation(long lobbyId, Map map)
        {
            var neededLobby = await _ctx.Lobbies
                .Where(l => l.Id
                    .Equals(lobbyId) && l.Status.Equals(LobbyStatus.Playing))
                .Include(l => l.Matches.Where(m => m.PlayedMap.Equals(map)))
                .ThenInclude(m => m.Stats)
                .ThenInclude(m => m.User)
                .ThenInclude(u => u!.Teams.Where(t => t.Lobby!.Status.Equals(LobbyStatus.Playing)))
                .FirstOrDefaultAsync() ??
                throw new ApplicationException(AppDictionary.JoinLobbyNotFound);

            return neededLobby;
        }

        public override async Task<Lobby?> GetLobbyForUpdating(long lobbyId)
        {
            var lobby = await _ctx.Lobbies.FirstOrDefaultAsync(l => l.Id.Equals(lobbyId) &&
                     l.Status.Equals(LobbyStatus.Configuring));
            return lobby;
        }
        private Expression<Func<GetMatchView, object?>> GetMatchOrderFunction(string orderProperty)
        {
            return orderProperty switch
            {
                "id" => u => u.Id,
                "server" => u => u.Server.Location,
                "path" => u => u.Server.Path,
                "players" => u => u.PlayersAmount,
                "status" => u => u.Status,
                _ => u => u.Id
            };
        }
        public async override Task<GetMatchView[]> GetMatchesForAdmin(GetMatchesForAdminRequest req)
        {
            var idSearchParam = default(long);
            _ = long.TryParse(req.SearchParam, out idSearchParam);
            using var ctx = await _factory.CreateDbContextAsync();
            var startQuery = ctx.Lobbies.AsQueryable();
            if (!string.IsNullOrEmpty(req.SearchParam))
            {
                if (idSearchParam != default)
                {
                    if (string.IsNullOrEmpty(req.FindBy))
                    {
                        startQuery = startQuery.Where(l => l.Id == idSearchParam || l.Teams
                        .SelectMany(t => t.Users).Select(u => u.Id).Contains(idSearchParam));
                    }
                    else if (req.FindBy == AppDictionary.FindById)
                    {
                        startQuery = startQuery.Where(l => l.Id == idSearchParam);
                    }
                    else
                    {
                        startQuery = startQuery.Where(l => l.Teams
                        .SelectMany(t => t.Users).Select(u => u.Id).Contains(idSearchParam));
                    }
                }
                else
                {
                    startQuery = startQuery.Where(l => l.Teams
                    .SelectMany(t => t.Users)
                    .Select(u => u.Name.ToLower()).Any(n => n.Contains(req.SearchParam.ToLower())));
                }
            }
            var orderKeySelector = GetMatchOrderFunction(req.OrderProperty);
            var query = startQuery.Where(s => s.Status != LobbyStatus.Configuring)
                .ProjectTo<GetMatchView>(_mapper.ConfigurationProvider);
            if (req.Order == "asc")
                query = query.OrderBy(orderKeySelector);
            else
                query = query.OrderByDescending(orderKeySelector);
            var result = await query
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToArrayAsync();
            return result;
        }

        public async override Task<long> GetStaledLobby(TimeSpan staleValue)
        {
            var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Lobbies
                .Where(s => s.LastServerUpdate + staleValue < DateTime.UtcNow &&
                (s.Status == LobbyStatus.Playing
                || s.Status == LobbyStatus.Warmup))
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }

        public override async Task<Team?> GetTeamForUpdating(long creatorId)
        {
            var team = await _ctx.Teams
                .FirstOrDefaultAsync(t => t.Lobby!.Status.Equals(LobbyStatus.Configuring)
                    && t.CreatorId.Equals(creatorId));
            return team;
        }

        public override Task<UserBid?> GetUserBid(long userId)
        {
            var userBid = _ctx.Bids
                .Include(b => b.Lobby)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId.Equals(userId) &&
                b.Lobby!.Status.Equals(LobbyStatus.Configuring));
            return userBid;
        }

        public override async Task<List<long>> GetUsersIdInLobbyAsync(long lobbyId)
        {
            var result = await _ctx.Lobbies
                .AsNoTrackingWithIdentityResolution()
                .Include(u => u.Teams)
                .ThenInclude(t => t.Users)
                .Where(l => l.Id.Equals(lobbyId))
                .SelectMany(l => l.Teams)
                .SelectMany(t => t.Users)
                .Select(u => u.Id)
                .ToListAsync();
            return result;
        }


        public override async Task<bool> UserAlreadyInLobby(long userId)
        {
            Expression<Func<Team, bool>> expression = u => u.Users.Select(t => t.Id).Contains(userId)
            && (u.Lobby!.Status.Equals(LobbyStatus.Veto) ||
            u.Lobby!.Status.Equals(LobbyStatus.Configuring) ||
            u.Lobby!.Status.Equals(LobbyStatus.Playing));
            var lobby = await _ctx.Teams
                .AsNoTrackingWithIdentityResolution()
                .Where(expression)
                .Select(t => t.Lobby)
                .FirstOrDefaultAsync();

            return lobby != null;
        }

        public override async Task<bool> UserIsCreatorOfLobby(long userId, long lobbyId)
        {
            var lobbyCreatorId = await _ctx.Lobbies
               .Where(l => l.Id.Equals(lobbyId))
               .Select(l => l.CreatorId)
               .FirstOrDefaultAsync();
            return lobbyCreatorId == userId;
        }

        public async override Task<int> GetLobbyCounts(string status)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var query = ctx.Lobbies.AsQueryable();
            if (status.Equals(AppDictionary.MatchPlaying))
                query = query.Where(s => s.Status == LobbyStatus.Playing);
            else if (status.Equals(AppDictionary.MatchCanceled))
                query = query.Where(s => s.Status == LobbyStatus.Canceled);
            else
                query = query.Where(s => s.Status == LobbyStatus.Over);
            return await query.CountAsync();
        }

        public async override Task<decimal> GetLobbyComissions(string interval)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var dateInterval = DateIntervals.GetDateInterval(interval);
            var finalBidsOverInterval = await ctx.Lobbies
                .Where(s => s.Status == LobbyStatus.Over
                && s.LastServerUpdate >= dateInterval.StartDate
                && s.LastServerUpdate <= dateInterval.EndDate)
                .SelectMany(s => s.Bids)
                .SumAsync(s => s.Bid);
            return finalBidsOverInterval * AppConfig.AmountOfComission;
        }

        public async override Task<GetUserDto[]> GetUsersInLobbyAsync(long lobbyId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Lobbies.Where(s => s.Id == lobbyId)
                .SelectMany(l => l.Teams)
                .SelectMany(t => t.Users)
                .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }
    }
}
