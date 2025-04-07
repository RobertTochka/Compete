
namespace compete_platform.Infrastructure.Middlewares
{
    public class ShowHeaders : IMiddleware
    {
        private static string _headers = string.Empty;
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(context.Request.Path.Value?.Contains("headers-show") ?? false)
            {
                _headers = string.Join("\n", context.Request.Headers.Select(s => $"{s.Key} - {s.Value}"));
                await context.Response.WriteAsync(_headers);
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
