
using compete_poco.Infrastructure.Data;

namespace compete_platform.Infrastructure.Middlewares
{
    public class GameServerAuthMiddleware : IMiddleware
    {
        private readonly AppConfig _cfg;

        public GameServerAuthMiddleware(AppConfig cfg) => _cfg = cfg;
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(context.Request.Path.Value is not null && context.Request.Path.Value.Contains("gameserver"))
            {
                var authToken = context.Request.Headers.Authorization.ToString();
                if (authToken is null)
                    authToken = string.Empty;
                authToken = authToken.Split(" ").Last();
                if(!_cfg.ServiceKey.Equals(authToken))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Not permissions");
                }
                else
                    await next(context);

            }
            else
                await next(context);
        }
    }
}
