using Microsoft.AspNetCore.Authorization;

namespace compete_platform.Infrastructure
{
    public class AdminClaimRequirement : IAuthorizationRequirement
    {
        public AdminClaimRequirement()
        {

        }
    }
    public class AdminClaimHandler : AuthorizationHandler<AdminClaimRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminClaimRequirement requirement)
        {
            if (context.User is null
                || context.User.Identity is null
                || !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }
            if (context.User.HasClaim(c => c.Type == "Admin" && c.Value == "True"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
