using compete_poco.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;

namespace compete_platform.Infrastructure.Services
{
    public abstract class CConfigRepository : CRepository
    {
        protected CConfigRepository(ApplicationContext ctx) : base(ctx)
        {
        }

        public abstract Task<Config?> GetConfigByName(string name);
        public abstract Task AddConfig(Config config);
    }
}
