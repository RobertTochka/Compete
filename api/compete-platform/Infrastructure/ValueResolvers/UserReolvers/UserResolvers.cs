using compete_poco.Infrastructure.Extensions;
using compete_poco.Models;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.ValueResolvers.UserReolvers
{
    public static class UserResolvers
    {
        public static Expression<Func<User, decimal>> ArreasByBids =>
            (User u) => u.Bids
            .Where(b => b.Lobby!.Status == LobbyStatus.Playing || b.Lobby.Status == LobbyStatus.Warmup)
            .Select(b => b.Bid)
            .Sum();
        public static Expression<Func<User, decimal>> ArreasByLosedAwards =>
           (User u) => u.Awards
           .Where(b => b.Award < 0 && b.PayTime == DateTime.MinValue)
           .Select(b => b.Award)
           .Sum();
        public static bool InLobby(User x) => x.Teams.FirstOrDefault(t => t.Users.Select(u => u.Id).Contains(x.Id)
                && t.Lobby!.IsCurrentLobby()) != null;
        public static int GetMatches(User x) 
        {
            return x.Stats.GroupBy(s => s.Match!.LobbyId).Count();
        }
        public static double GetWinrate(User x)
        {
            var allMatches = x.Stats.GroupBy(s => s.Match!.LobbyId).Count();
            return x.Stats.Where(s =>
            s.Match!.Lobby!.TeamWinner != null && 
            x.Teams.Select(t => t.Id).Contains((long)s.Match!.Lobby!.TeamWinner))
                .GroupBy(s => s.Match!.LobbyId).Count() / (allMatches.Equals(0) ? 1 : allMatches);
        }
        public static float GetKD(User x)
        {
            var deaths = x.Stats.Sum(x => x.Deaths);
            return x.Stats.Sum(s => s.Kills) / (deaths.Equals(0) ? 1 : deaths);
        }
        
    }
}
