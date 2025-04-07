using compete_platform.Dto;
using compete_poco.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;

namespace compete_platform.Infrastructure.Services
{
    public abstract class CPlatformEventRepository : CRepository
    {
        protected CPlatformEventRepository(ApplicationContext ctx) : base(ctx)
        {
        }
        public abstract Task<List<PlatformEvent>> GetLastPlatformEvents(GetBatchOfPagedEntitiesRequest req);
    }
}
