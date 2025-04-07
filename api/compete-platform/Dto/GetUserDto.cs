using compete_platform.Dto.Common;
using compete_poco.Models;
using System.Runtime.CompilerServices;

namespace compete_poco.Dto
{
    public class GetUserDto : ISteamUserBasedDto<GetUserDto>, 
        IStatsRatedUser, 
        IRatedUser,
        IMatchableUser
    {
        public long Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime RegistrationDate { get; set; }
        public long? TeamId { get; set; }
        public bool IsOnline { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CanInvite { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public bool InLobby { get; set; }
        public string SteamId { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime LastSteamInfoUpdate { get; set; }
        public List<GetUserDto> Friends { get; set; } = new();
        public double HeadshotPercent { get; set; }
        public decimal Income { get; set; }
        public float KillsByDeaths { get; set; }
        public long? RatePlace { get; set; }
        public double Winrate { get; set; }
        public double Rate { get; set; }
        public int Matches { get; set; }
        public List<string> LastResults { get; set; } = new();
    }
}
