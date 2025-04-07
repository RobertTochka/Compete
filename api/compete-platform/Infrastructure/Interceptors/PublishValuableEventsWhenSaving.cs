using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace compete_platform.Infrastructure.Interceptors
{
    public class PublishValuableEventsWhenSaving : SaveChangesInterceptor
    {
        public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            DbContext? context = eventData.Context;
            if (context == null) 
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            var platformEvents = new List<PlatformEvent>();
            foreach (var entitysWithEvent in context.ChangeTracker
                .Entries<IContainsValuableEvents>()
                .Select(s => s.Entity)
                .Where(s => s.GetEventPayload() != null)
                .DistinctBy(c => c.GetEventPayload()))
            {
                PlatformEvent platformEvent = new()
                {
                    OcurredOnUtc = DateTime.UtcNow,
                    Payload = entitysWithEvent.GetEventPayload()!
                };
                platformEvents.Add(platformEvent);
            }
            await context.Set<PlatformEvent>().AddRangeAsync(platformEvents, cancellationToken);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
