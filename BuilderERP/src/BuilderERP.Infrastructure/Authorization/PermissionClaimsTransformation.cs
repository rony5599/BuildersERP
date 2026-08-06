using System.Security.Claims;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Authorization;

public class PermissionClaimsTransformation : IClaimsTransformation
{
    private readonly AppDbContext _context;

    public PermissionClaimsTransformation(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity is null || !identity.IsAuthenticated)
        {
            return principal;
        }

        if (identity.HasClaim(c => c.Type == PermissionClaimTypes.Permission))
        {
            return principal;
        }

        var roleNames = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (roleNames.Count == 0)
        {
            return principal;
        }

        var permissionNames = await _context.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();

        foreach (var permissionName in permissionNames)
        {
            identity.AddClaim(new Claim(PermissionClaimTypes.Permission, permissionName));
        }

        return principal;
    }
}
