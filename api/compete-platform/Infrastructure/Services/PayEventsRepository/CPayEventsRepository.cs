using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_poco.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using System.Diagnostics.Tracing;

namespace compete_platform.Infrastructure.Services.PayEventsRepository
{
    public abstract class CPayEventsRepository : CRepository
    {
        public CPayEventsRepository(ApplicationContext ctx) : base(ctx)
        {
        }
        public abstract Task<PayEvent> CreateEvent(PayEvent e);
        public abstract Task<List<GetPayEvent>> GetPayEvents(GetBatchOfPagedEntitiesRequest req);
        public abstract Task<decimal> GetFinancialRotationByInterval(string interval);
    }
}
