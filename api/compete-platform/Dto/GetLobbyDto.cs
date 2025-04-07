using compete_platform.Dto;
using compete_poco.Models;

namespace compete_poco.Dto
{
    public class GetLobbyDto : ICloneable
    {
        public GetLobbyDto()
        {
            Awards = new();
            Bids = new List<GetUserBidDto>();
            PickMaps = new List<Map>();
            Teams = new List<GetTeamDto>();
            MapActions = new();
            Matches = new();
        }
        public long Id { get; set; }
        public long CreatorId { get; set; }
        public List<Map> PickMaps { get; set; }
        public bool Public { get; set; }
        public int ServerId { get; set; }
        public Guid CodeToConnect { get; set; }
        public GetServerDto Server { get; set; } = null!;
        public List<GetMatchDto> Matches { get; set; }
        public long ChatId { get; set; }
        public List<GetUserAwardDto> Awards { get; set; }
        public long? TeamWinner { get; set; }
        public List<GetUserBidDto> Bids { get; set; }
        public List<GetTeamDto> Teams { get; set; }
        public ServerConfig Config { get; set; } = null!;
        public List<MapActionInfo> MapActions { get; set; }
        public LobbyStatus Status { get; set; }
        public Models.Type PlayersAmount { get; set; }
        public Format MatchFormat { get; set; }
        public int? Port { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
