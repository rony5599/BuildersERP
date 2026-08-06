using BuilderERP.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Company>().HasIndex(c => c.Code).IsUnique();
        builder.Entity<Branch>().HasIndex(b => new { b.CompanyId, b.Code }).IsUnique();
        builder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();

        // Restrict deletes through the reference hierarchy so SQL Server doesn't have to
        // reconcile multiple cascade paths down to AspNetUsers (Company -> Branch -> User).
        builder.Entity<Branch>()
            .HasOne(b => b.Company)
            .WithMany(c => c.Branches)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Department>()
            .HasOne(d => d.Branch)
            .WithMany(b => b.Departments)
            .HasForeignKey(d => d.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Project>()
            .HasOne(p => p.Branch)
            .WithMany(b => b.Projects)
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CostCenter>()
            .HasOne(cc => cc.Project)
            .WithMany(p => p.CostCenters)
            .HasForeignKey(cc => cc.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Branch)
            .WithMany()
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        // Permission is reference/lookup data referenced as a required end of RolePermission;
        // it is excluded from the soft-delete filter so EF doesn't warn about filtering out
        // a required navigation (permissions are hard-deleted, not soft-deleted).
        var softDeleteEntities = new[] { typeof(Company), typeof(Branch), typeof(Project), typeof(Department), typeof(CostCenter) };
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (softDeleteEntities.Contains(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }
}
