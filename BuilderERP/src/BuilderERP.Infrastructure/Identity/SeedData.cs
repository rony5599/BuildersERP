using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Identity;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        await context.Database.MigrateAsync();

        foreach (var roleName in new[] { RoleNames.SuperAdmin, RoleNames.Admin, RoleNames.Employee })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName, Description = $"{roleName} role" });
            }
        }

        var existingPermissions = await context.Permissions.ToDictionaryAsync(p => p.Name);
        foreach (var (name, module) in PermissionNames.All)
        {
            if (!existingPermissions.ContainsKey(name))
            {
                var permission = new Permission { Name = name, Module = module };
                context.Permissions.Add(permission);
                existingPermissions[name] = permission;
            }
        }
        await context.SaveChangesAsync();

        var superAdminRole = await roleManager.FindByNameAsync(RoleNames.SuperAdmin);
        var adminRole = await roleManager.FindByNameAsync(RoleNames.Admin);

        await GrantPermissionsAsync(context, superAdminRole!.Id, PermissionNames.All.Select(p => p.Name), existingPermissions);
        await GrantPermissionsAsync(context, adminRole!.Id, PermissionNames.AdminGrants, existingPermissions);
        await context.SaveChangesAsync();

        var company = await context.Companies.FirstOrDefaultAsync(c => c.Code == "DEFAULT");
        if (company is null)
        {
            company = new Company { Name = "Default Company", Code = "DEFAULT" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }

        const string adminEmail = "admin@builderERP.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                CompanyId = company.Id
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoleNames.SuperAdmin);
            }
        }
    }

    private static async Task GrantPermissionsAsync(AppDbContext context, Guid roleId, IEnumerable<string> permissionNames, IReadOnlyDictionary<string, Permission> permissionsByName)
    {
        var existingGrants = await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (var name in permissionNames)
        {
            var permission = permissionsByName[name];
            if (!existingGrants.Contains(permission.Id))
            {
                context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permission.Id });
            }
        }
    }
}
