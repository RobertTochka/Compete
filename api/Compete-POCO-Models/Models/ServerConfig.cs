using Microsoft.EntityFrameworkCore;

namespace compete_poco.Models
{
    [Owned]
    public class ServerConfig
    {
        public bool FriendlyFire { get; set; }
        public int FreezeTime { get; set; }
    }
}