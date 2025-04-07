using compete_platform.Dto.Common;

namespace compete_platform.Dto
{
    public class GetUserRateDto : ISteamUserBasedDto<GetUserRateDto>, 
        IStatsRatedUser, IRatedUser, IMatchableUser
    {
        public long Id { get; set; }
        public string SteamId { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public List<GetUserRateDto> Friends { get; set; } = new();
        public double HeadshotPercent { get; set; }
        public decimal Income { get; set; }
        public float KillsByDeaths { get; set; }
        public long? RatePlace { get; set; }
        public double Winrate { get; set; }
        public double Rate { get; set; }
        public bool IsOnline { get; set; }
        public int Matches { get; set; }
        public List<string> LastResults { get; set; } = new();
    }
}
