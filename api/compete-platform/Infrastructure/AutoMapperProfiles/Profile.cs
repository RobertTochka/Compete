using AutoMapper;
using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_platform.Dto.Common;
using compete_platform.Infrastructure.Services.AggregationFunctions;
using compete_platform.Infrastructure.ValueResolvers;
using compete_poco.Dto;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.LobbyService.Models;
using compete_poco.Infrastructure.ValueResolvers.UserReolvers;
using compete_poco.Models;
using Compete_POCO_Models;
using Compete_POCO_Models.Models;
using CompeteGameServerHandler.Dto;
using CompeteGameServerHandler.Infrastructure.PropertyResolvers;

namespace compete_poco.Infrastructure.AutoMapperProfiles;

public class AutoProfile : Profile
{
    public AutoProfile() 
    {
        CreateMap<User, IRatedUser>()
             .ForMember(x => x.Winrate, cg => cg.MapFrom(UserFunctions.CountWinrate))
             .IncludeAllDerived();

        CreateMap<User, IStatsRatedUser>()
             .ForMember(x => x.HeadshotPercent, cfg => cfg.
                MapFrom(UserFunctions.CountHeadshotPercentage))
            .ForMember(x => x.KillsByDeaths, cfg => cfg.MapFrom(UserFunctions.CountKD))
            .ForMember(x => x.Income, cfg => cfg.MapFrom(UserFunctions.CountIncome))
        .IncludeAllDerived();
        
        CreateMap<User, IMatchableUser>()
            .ForMember(x => x.Matches, cfg => cfg.
                MapFrom(UserFunctions.CountMatches))
            .ForMember(x => x.LastResults, cfg => cfg.MapFrom(UserFunctions.CountLastResults))
            .IncludeAllDerived();

        CreateMap<User, GetUserDto>();

        CreateMap<User, GetUserRateDto>();

        CreateMap<User, GetUserView>()
            .ForMember(x => x.Profit, cfg => cfg.MapFrom(UserFunctions.CountProfit))
            .ForMember(x => x.CurrentLobby, cfg => cfg.MapFrom(UserFunctions.GetCurrentLobby));


        CreateMap<Lobby, GetLobbyDto>()
            .ForMember(x => x.Port, cfg => cfg.MapFrom(LobbyViewFunctions.DefinePort));
        CreateMap<Lobby, GetMatchView>();
        CreateMap<Pay, GetPayDto>()
            .ForMember(x => x.CreationTime, cfg => cfg.MapFrom(
                y => DateTimeToRuFormatStringConverters.Convert(y.CreationTime)));
        CreateMap<Team, GetTeamDto>();
        CreateMap<UserBid, GetUserBidDto>();
        CreateMap<UserAward, GetUserAwardDto>();
        CreateMap<Server, GetServerDto>();
        CreateMap<ChatMessage, GetChatMessageDto>();
        CreateMap<AppealChatMessage, GetChatMessageDto>();
        CreateMap<Lobby, GetLobbyViewDto>()
            .ForMember(x => x.Capacity, x => x.MapFrom(LobbyViewFunctions.CountCapacity))
            .ForMember(x => x.BankSumm, x => x.MapFrom(LobbyViewFunctions.CountBaknSumm));

        CreateMap<LobbyAdminConfiguration, Lobby>();
        CreateMap<SendInviteRequest, JoinToLobbyInfo>();
        CreateMap<Match, GetMatchDto>();
        CreateMap<Server, GetServerDto>();
        CreateMap<PlayerLogInformation, UserStat>()
            .ForMember(x => x.Headshots, cfg => cfg.MapFrom(UserStatResolvers.CountHeadshots));
        CreateMap<MapActionInfo, MapInformation>()
            .ForMember(x => x.Map, opt => opt.MapFrom(x => GameServerResolvers.ConvertMapToString(x.Map)));
        CreateMap<Team, TeamInformation>()
            .ForMember(x => x.SteamIds, cfg => cfg.MapFrom(GameServerResolvers.GetSteamIds))
            .ForMember(x => x.CreatorSteamId, cfg => cfg.MapFrom(GameServerResolvers.GetCreatorSteamId));
        CreateMap<User, UserArreasInfo>()
            .ForMember(x => x.ArreasByLoseAward, cfg => cfg.MapFrom(UserResolvers.ArreasByLosedAwards))
            .ForMember(x => x.ArreasByAvailableBids, cfg => cfg.MapFrom(UserResolvers.ArreasByBids));
        CreateMap<Lobby, InitialConfiguration>()
            .ForMember(x => x.Cfg, cfg => cfg.MapFrom(x => x.Config))
            .ForMember(x => x.PlayingMaps, cfg => cfg.MapFrom(x => x.MapActions));

        CreateMap<PayEvent, GetPayEvent>()
            .ForMember(x => x.EventType, cfg => cfg.MapFrom(PayResolvers.GetEventType))
            .ForMember(x => x.Status, cfg => cfg.MapFrom(PayResolvers.MapEventToStatus));
        CreateMap<Report, GetReportDto>();
        CreateMap<CreateReportDto, Report>();
    }
}
