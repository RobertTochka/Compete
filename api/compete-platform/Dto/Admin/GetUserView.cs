using compete_platform.Dto.Common;

namespace compete_platform.Dto.Admin
{
    public class GetUserView : ISteamUserBasedDto<GetUserView>,
        IRatedUser, IMatchableUser
    {
        public long Id { get; set; }
        public string SteamId { get; set; } = null!;
        public decimal Balance { get; set; }
        public decimal Profit { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public List<GetUserView> Friends { get; set; } = new();
        public long? RatePlace { get; set; }
        public double Winrate { get; set; }
        public double Rate { get; set; }
        public bool IsOnline { get; set; }
        public int Matches { get; set; }
        public bool IsBanned { get; set; }
        public long? CurrentLobby {  get; set; }
        public List<string> LastResults { get; set; } = new();
    }
}
