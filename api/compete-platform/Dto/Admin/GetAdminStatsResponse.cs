namespace compete_platform.Dto.Admin
{
    public class GetAdminStatsResponse
    {
        public int UsersCount { get; set; }
        public int ServersCount { get; set; }
        public decimal FinancialRotation { get; set; }
        public int MatchesCount { get; set; }
        public decimal LobbyComissions { get; set; }
    }
}
