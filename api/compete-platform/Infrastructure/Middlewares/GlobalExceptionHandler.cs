using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace compete_poco.Infrastructure.Middlewares
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
        {
            _logger = logger;
        }
        private async Task HandleError(HttpContext context, string? title, string detail, int statusCode)
        {
         
            var problemDetails = new ProblemDetails();
            problemDetails.Status = statusCode;
            if(title is not null)
                problemDetails.Title = title;
            problemDetails.Detail = detail;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
            await context.Response.Body.FlushAsync();
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(ApplicationException e)
            {
                await HandleError(context, null, e.Message, StatusCodes.Status400BadRequest);
            }
            catch(DbUpdateConcurrencyException)
            {
                await HandleError(context, null, AppDictionary.ConcurrencyUpdateError, StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\nPath {context.Request.Path}");
                if (e.InnerException != null)
                    _logger.LogError(e.InnerException.Message);
                await HandleError(context, 
                    AppDictionary.ServerErrorOcurred, 
                    e.Message, 
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}
