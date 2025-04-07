using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace compete_poco.Infrastructure.Data
{
    public class AppConfig
    {
        public string SqlKey { get; set; } = string.Empty;
        public string SteamApiKey { get; set; } = string.Empty;
        public string ServiceKey { get; set; } = string.Empty;
        public string Host { get; set; } = null!;
        public string? TestSqlKey {  get; set; }
        public static string RelativeFilePath => "/api/files";
        public string Redis { get; set; } = string.Empty;   
        public string JwtKey { get; set; } = string.Empty;
        public static decimal AmountOfComission { get; set; } = 0.1M;
        public string Issuer { get; set; } = string.Empty;
        public SymmetricSecurityKey JwtKeyObject => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        public TimeSpan CredentialsAvailabilityTime => TimeSpan.FromDays(7);
        public string ServerManagingAccessKey { get; set; } = null!;
        public short MaxAmountAvailablePorts { get; set; } = 14;
        public string PaymentKey { get; set; } = null!;
        public string ShopId { get; set; } = null!;
        public string AgentId { get; set; } = null!;
        public string PayoutKey { get; set; } = null!;
        public int CsServerManagingApiPort { get; } = 3777;
        public TimeSpan MapInitialWarmupTime => TimeSpan.FromSeconds(270);
        public static TimeSpan MapInitialWarmupTimeGlobally => TimeSpan.FromSeconds(270);
        public static TimeSpan FrequencyOfSteamUserRefreshing => TimeSpan.FromDays(1);
        public static TimeSpan FrequencyOfRaitingUpdating => TimeSpan.FromHours(1);
        public static DateTime LastTimeOfRatingUpdate { get; set; }
        public static TimeSpan FrequencyOfServersHealthyChecking => TimeSpan.FromMinutes(1);
        public static int MaxAmountOfReportForLobby => 3;
    }
}
