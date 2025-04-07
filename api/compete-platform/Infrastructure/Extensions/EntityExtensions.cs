using compete_poco.Models;
using System.Linq.Expressions;

namespace compete_poco.Infrastructure.Extensions
{
   public static class LobbyExtensions
    {
        public static bool IsCurrentLobby(this Lobby lobby)
        {
            return
            lobby!.Status.Equals(LobbyStatus.Veto) ||
           lobby!.Status.Equals(LobbyStatus.Configuring) ||
           lobby!.Status.Equals(LobbyStatus.Playing) ||
           lobby!.Status.Equals(LobbyStatus.Warmup);
        }
    }
}
