using BuilderERP.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BuilderERP.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim(PermissionClaimTypes.Permission, requirement.PermissionName))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
