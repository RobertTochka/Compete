using compete_platform.Dto;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;


namespace compete_platform.Infrastructure.Services
{
    public class PlatformEventRepository : CPlatformEventRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _factory;

        public PlatformEventRepository(ApplicationContext ctx, 
            IDbContextFactory<ApplicationContext> factory) : base(ctx)
        {
            _factory = factory;
        }

        public async override Task<List<PlatformEvent>> GetLastPlatformEvents(
            GetBatchOfPagedEntitiesRequest req)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.PlatformEvents
                .OrderByDescending(S => S.OcurredOnUtc)
                .Skip(req.PageSize * (req.Page - 1))
                .Take(req.PageSize)
                .ToListAsync();
            return result;
        }
    }
}
