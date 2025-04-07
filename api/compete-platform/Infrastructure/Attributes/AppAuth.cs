using compete_poco.Infrastructure.Services.TokenProvider;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace compete_platform.Infrastructure
{
    public class AppAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private string? role { get; }
        public AppAuthAttribute(string? requiredRole = null)
        {
            role = requiredRole;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var services = context.HttpContext.RequestServices;
            using var scope = services.CreateScope();
            var tokenProvider = scope.ServiceProvider.GetRequiredService<ITokenProvider>();
            var authCookie = context.HttpContext.Request.Cookies[AppDictionary.AppIdentity];
            if (authCookie == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var result = await tokenProvider.VerifyTokenAsync(authCookie);
            if (!result)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var token = tokenProvider.ReadToken(authCookie);
            if (!string.IsNullOrEmpty(role) && role == AppDictionary.Admin
                && !token.Claims.Any(c => c.Type == AppDictionary.Admin && c.Value == "True"))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
