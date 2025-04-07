using compete_poco.Models;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Services
{
    public static class UserFunctions
    {
        public static Expression<Func<User, long?>> GetCurrentLobby =>
            u => u.Teams.Where(t =>
            t.Lobby!.Status == LobbyStatus.Playing
            || t.Lobby.Status == LobbyStatus.Warmup).Select(s => s.LobbyId).FirstOrDefault();
        public static Expression<Func<User, float>> CountKD =>
            u => u.Stats.Sum(s => s.Kills) / (float)(u.Stats.Sum(s => s.Deaths) + 10e-6);
        public static Expression<Func<User, double>> CountWinrate =>
            u => u.Teams.Where(t => t.Lobby!.TeamWinner == t.Id).Count() /
            (float)(u.Teams.Where(t => t.Lobby!.Status == LobbyStatus.Over).Count() + 10e-6) * 100;
        public static Expression<Func<User, int>> CountMatches =>
            u => u.Teams.Where(t => t.Lobby!.Status.Equals(LobbyStatus.Over)).Count();
        public static Expression<Func<User, List<string>>> CountLastResults =>
            u => u.Teams
            .Where(t => t.Lobby!.Status == LobbyStatus.Over)
            .OrderBy(t => t.Lobby!.CreateTime)
            .Select(t => t.Lobby!.TeamWinner == t.Id ? "W" : "L")
            .ToList();
        public static Expression<Func<User, double>> CountHeadshotPercentage =>
            u => u.Stats.Sum(s => s.Headshots) / (float)(u.Stats.Sum(s => s.Kills) + 10e-6) * 100;
        public static Expression<Func<User, decimal>> CountIncome =>
            u => u.Awards.Where(a => a.AwardType == AwardType.Win).Sum(a => a.Award);
        public static Expression<Func<User, decimal>> CountProfit =>
            u => u.Awards.Sum(a => a.Award);
    }
}
