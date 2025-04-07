using Microsoft.AspNetCore.SignalR;

namespace compete_poco.Hubs
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.Claims.First(c => c.Type.Equals("Id")).Value;
        }
    }
}
