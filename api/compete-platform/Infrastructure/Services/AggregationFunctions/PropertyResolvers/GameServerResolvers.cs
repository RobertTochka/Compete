using compete_poco.Models;
using System.Linq.Expressions;

namespace CompeteGameServerHandler.Infrastructure.PropertyResolvers
{
    public class GameServerResolvers
    {
        public static Expression<Func<Team, List<long>>> GetSteamIds => t 
            => t.Users.Select(u => long.Parse(u.SteamId)).ToList();
        public static Expression<Func<Team, long>> GetCreatorSteamId => t
            => long.Parse(t.Users.First(u => u.Id.Equals(t.CreatorId)).SteamId);
        public static string ConvertMapToString(Map map)
        {
            switch (map)
            {
                case Map.Mirage:
                    return "de_mirage";
                case Map.Inferno:
                    return "de_inferno";
                case Map.Nuke:
                    return "de_nuke";
                case Map.Anubis:
                    return "de_anubis";
                case Map.Overpass:
                    return "de_overpass";
                case Map.Vertigo:
                    return "de_vertigo";
                case Map.Ancient:
                    return "de_ancient";
                case Map.Dust2:
                    return "de_dust2";
                case Map.Office:
                    return "cs_office";
                case Map.Italy:
                    return "cs_italy";
                default:
                    throw new ArgumentOutOfRangeException(nameof(map), map, null);
            }
        }
    }
}
