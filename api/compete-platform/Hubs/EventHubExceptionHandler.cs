using compete_platform.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection;

namespace compete_poco.Hubs
{
    public class EventHubExceptionHandler : IHubFilter
    {
        private ILogger<EventHubExceptionHandler> _logger = null!;
        static EventHubExceptionHandler()
        {
           
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var eventHub = (invocationContext.Hub as EventHub)!;
            _logger = invocationContext.ServiceProvider.GetRequiredService<ILogger<EventHubExceptionHandler>>();
            try
            {
                return await next(invocationContext);
            }
            catch(ApplicationException ex)
            {
                await ErrorHandlers.HandleEventHubError(ex.Message, eventHub);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);
                await ErrorHandlers.HandleEventHubError(AppDictionary.ConcurrencyUpdateError, eventHub);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await ErrorHandlers.HandleEventHubError(AppDictionary.ServerErrorOcurred, eventHub);
            }
            return Task.CompletedTask;
        }
    }
}
