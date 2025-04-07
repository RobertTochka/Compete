using AutoMapper;
using compete_platform.Dto;
using compete_platform.Dto.Admin;
using compete_platform.Infrastructure.ValueResolvers;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace compete_platform.Infrastructure.Services.PayEventsRepository
{
    public class PayEventRepository : CPayEventsRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _factory;
        private readonly IMapper _mapper;

        public PayEventRepository(ApplicationContext ctx, 
            IDbContextFactory<ApplicationContext> factory,
            IMapper mapper) : base(ctx)
        {
            _factory = factory;
            _mapper = mapper;
        }

        public async override Task<PayEvent> CreateEvent(PayEvent e)
        {
            var prevState = await _ctx.PayEvents
                .FirstOrDefaultAsync(a => a.CorrelationId == e.CorrelationId);
            if (prevState is not null)
                _ctx.PayEvents.Remove(prevState);
            await _ctx.PayEvents.AddAsync(e);
            return e;
        }
        private static Expression<Func<PayEvent, object?>> GetPayEventsOrderFunction(string orderProp)
        {
            return orderProp switch
            {
                "id" => e => e.Id,
                "date" => e => e.CreatedUtc,
                "state" => e => e.PayState,
                _ => e => e.Id
            };
        }
        public async override Task<List<GetPayEvent>> GetPayEvents(GetBatchOfPagedEntitiesRequest req)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var orderKeyFunction = GetPayEventsOrderFunction(req.OrderProperty);
            var lastPayEvents = ctx.PayEvents.AsQueryable();
            if (req.Order == "asc")
                lastPayEvents = lastPayEvents.OrderBy(orderKeyFunction);
            else
                lastPayEvents = lastPayEvents.OrderByDescending(orderKeyFunction);
            var result = await lastPayEvents
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();
            return _mapper.Map<List<GetPayEvent>>(result
                .GroupBy(e => e.CorrelationId)
                .Select(g => g.MaxBy(e => e.PayState)));
                
        }

        public async override Task<decimal> GetFinancialRotationByInterval(string interval)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var intervalDate = DateIntervals.GetDateInterval(interval);
            var financialRotation = await ctx.PayEvents
                .Where(s => (s.PayState == PayState.TopUpSuccess
                || s.PayState == PayState.RequestPayoutSuccess)
                && s.CreatedUtc <= intervalDate.EndDate
                && s.CreatedUtc >= intervalDate.StartDate)
                .Select(S => S.Amount)
                .SumAsync();
            return financialRotation;
        }
    }
}
