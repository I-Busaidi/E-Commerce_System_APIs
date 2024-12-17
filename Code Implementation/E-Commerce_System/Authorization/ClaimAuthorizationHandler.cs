using Microsoft.AspNetCore.Authorization;

namespace E_Commerce_System.Authorization
{
    public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            var claim = context.User.Claims.FirstOrDefault(c => c.Type == requirement.claimType);

            if (claim != null && claim.Value == requirement.claimValue)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
