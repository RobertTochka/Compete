using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;


namespace compete_platform.Infrastructure.Services
{
    public class ConfigRepository : CConfigRepository
    {

        public ConfigRepository(ApplicationContext ctx) : base(ctx)
        {
        }

        public async override Task AddConfig(Config config)
        {
            await _ctx.Configs.AddAsync(config);
        }

        public async override Task<Config?> GetConfigByName(string name)
        {
            return await _ctx.Configs.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
