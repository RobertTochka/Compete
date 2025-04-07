namespace compete_platform.Dto.Admin
{
    public class GetAdminStatsRequest
    {
        public string? UserInterval { get; set; }
        public bool? IsOnlineUsers { get; set; }
        public bool? OnlyHealthyServers { get; set; }
        public string FinancialRotationInterval { get; set; } = null!;
        public string MatchesStatus { get; set; } = null!;
        public string LobbyComissionsInterval { get; set; } = null!;
    }
}
