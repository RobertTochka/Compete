using compete_poco.Models;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Services
{
    public static class LobbyViewFunctions
    {
        public static Expression<Func<Lobby, int>> CountCapacity =>
            y => y.Teams
                    .OrderBy(t => t.Users.Count)
                        .First().Users.Count;
        public static Expression<Func<Lobby, decimal>> CountBaknSumm => y => y.Bids.Sum(t => t.Bid);
        public static Expression<Func<Lobby, int?>> DefinePort =>
            y => y.Server!.PlayingPorts.Where(pp => pp.LobbyId.Equals(y.Id)).Select(pp => pp.Port).FirstOrDefault();
    }
}
