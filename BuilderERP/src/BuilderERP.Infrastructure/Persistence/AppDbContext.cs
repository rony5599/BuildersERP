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
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Tower> Towers => Set<Tower>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<PropertyUnit> PropertyUnits => Set<PropertyUnit>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<SaleAgreement> SaleAgreements => Set<SaleAgreement>();

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

        builder.Entity<Building>()
            .HasOne(b => b.Project)
            .WithMany()
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Tower>()
            .HasOne(t => t.Building)
            .WithMany(b => b.Towers)
            .HasForeignKey(t => t.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Floor>()
            .HasOne(f => f.Tower)
            .WithMany(t => t.Floors)
            .HasForeignKey(f => f.TowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PropertyUnit>()
            .HasOne(u => u.Floor)
            .WithMany(f => f.Units)
            .HasForeignKey(u => u.FloorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PropertyUnit>().Property(u => u.Area).HasPrecision(18, 2);
        builder.Entity<PropertyUnit>().Property(u => u.Price).HasPrecision(18, 2);

        builder.Entity<Inquiry>()
            .HasOne(i => i.Lead)
            .WithMany()
            .HasForeignKey(i => i.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Inquiry>()
            .HasOne(i => i.PropertyUnit)
            .WithMany()
            .HasForeignKey(i => i.PropertyUnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<FollowUp>()
            .HasOne(f => f.Lead)
            .WithMany()
            .HasForeignKey(f => f.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Customer>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Customer>()
            .HasOne(c => c.Lead)
            .WithMany()
            .HasForeignKey(c => c.LeadId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Lead>()
            .HasOne(l => l.AssignedToUser)
            .WithMany()
            .HasForeignKey(l => l.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Quotation>()
            .HasOne(q => q.Customer)
            .WithMany()
            .HasForeignKey(q => q.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Quotation>()
            .HasOne(q => q.PropertyUnit)
            .WithMany()
            .HasForeignKey(q => q.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Quotation>().Property(q => q.QuotedPrice).HasPrecision(18, 2);

        builder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(b => b.PropertyUnit)
            .WithMany()
            .HasForeignKey(b => b.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>().Property(b => b.BookingAmount).HasPrecision(18, 2);

        builder.Entity<SaleAgreement>()
            .HasOne(a => a.Booking)
            .WithMany()
            .HasForeignKey(a => a.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SaleAgreement>().Property(a => a.TotalSalePrice).HasPrecision(18, 2);

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
        var softDeleteEntities = new[]
        {
            typeof(Company), typeof(Branch), typeof(Project), typeof(Department), typeof(CostCenter),
            typeof(Building), typeof(Tower), typeof(Floor), typeof(PropertyUnit),
            typeof(Lead), typeof(Inquiry), typeof(FollowUp), typeof(Customer),
            typeof(Quotation), typeof(Booking), typeof(SaleAgreement)
        };
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
