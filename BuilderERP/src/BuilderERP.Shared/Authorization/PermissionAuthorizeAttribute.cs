using Microsoft.AspNetCore.Authorization;

namespace BuilderERP.Shared.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string permissionName)
    {
        Policy = PermissionClaimTypes.PolicyPrefix + permissionName;
    }
}
