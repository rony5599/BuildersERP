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
    public DbSet<InstallmentPlan> InstallmentPlans => Set<InstallmentPlan>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseRequisition> PurchaseRequisitions => Set<PurchaseRequisition>();
    public DbSet<Rfq> Rfqs => Set<Rfq>();
    public DbSet<VendorQuotation> VendorQuotations => Set<VendorQuotation>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<GoodsReceive> GoodsReceives => Set<GoodsReceive>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockIssue> StockIssues => Set<StockIssue>();
    public DbSet<StockReturn> StockReturns => Set<StockReturn>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<WbsTask> WbsTasks => Set<WbsTask>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<BoqItem> BoqItems => Set<BoqItem>();
    public DbSet<DailyProgress> DailyProgresses => Set<DailyProgress>();
    public DbSet<SitePhoto> SitePhotos => Set<SitePhoto>();
    public DbSet<DelayEvent> DelayEvents => Set<DelayEvent>();
    public DbSet<BudgetLine> BudgetLines => Set<BudgetLine>();
    public DbSet<Drawing> Drawings => Set<Drawing>();
    public DbSet<DrawingRevision> DrawingRevisions => Set<DrawingRevision>();
    public DbSet<DrawingApproval> DrawingApprovals => Set<DrawingApproval>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<RateContract> RateContracts => Set<RateContract>();
    public DbSet<RunningBill> RunningBills => Set<RunningBill>();
    public DbSet<SecurityDeposit> SecurityDeposits => Set<SecurityDeposit>();
    public DbSet<PerformanceEvaluation> PerformanceEvaluations => Set<PerformanceEvaluation>();
    public DbSet<ContractorLedger> ContractorLedgers => Set<ContractorLedger>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<SafetyTraining> SafetyTrainings => Set<SafetyTraining>();
    public DbSet<Overtime> Overtimes => Set<Overtime>();
    public DbSet<Salary> Salaries => Set<Salary>();

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

        builder.Entity<InstallmentPlan>()
            .HasOne(p => p.SaleAgreement)
            .WithMany()
            .HasForeignKey(p => p.SaleAgreementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InstallmentPlan>().Property(p => p.TotalAmount).HasPrecision(18, 2);
        builder.Entity<InstallmentPlan>().Property(p => p.InterestRatePercent).HasPrecision(5, 2);

        builder.Entity<Installment>()
            .HasOne(i => i.InstallmentPlan)
            .WithMany(p => p.Installments)
            .HasForeignKey(i => i.InstallmentPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Installment>().Property(i => i.DueAmount).HasPrecision(18, 2);
        builder.Entity<Installment>().Property(i => i.PenaltyAmount).HasPrecision(18, 2);
        builder.Entity<Installment>().Property(i => i.PaidAmount).HasPrecision(18, 2);

        builder.Entity<Receipt>()
            .HasOne(r => r.Installment)
            .WithMany()
            .HasForeignKey(r => r.InstallmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Receipt>().Property(r => r.AmountPaid).HasPrecision(18, 2);

        builder.Entity<PurchaseRequisition>()
            .HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseRequisition>().Property(r => r.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<Rfq>()
            .HasOne(q => q.PurchaseRequisition)
            .WithMany()
            .HasForeignKey(q => q.PurchaseRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rfq>()
            .HasOne(q => q.Supplier)
            .WithMany()
            .HasForeignKey(q => q.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VendorQuotation>()
            .HasOne(v => v.Rfq)
            .WithMany(q => q.VendorQuotations)
            .HasForeignKey(v => v.RfqId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VendorQuotation>().Property(v => v.QuotedAmount).HasPrecision(18, 2);

        builder.Entity<PurchaseOrder>()
            .HasOne(o => o.VendorQuotation)
            .WithMany()
            .HasForeignKey(o => o.VendorQuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrder>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrder>().Property(o => o.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<GoodsReceive>()
            .HasOne(g => g.PurchaseOrder)
            .WithMany(o => o.GoodsReceives)
            .HasForeignKey(g => g.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceive>().Property(g => g.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<PurchaseReturn>()
            .HasOne(r => r.GoodsReceive)
            .WithMany()
            .HasForeignKey(r => r.GoodsReceiveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseReturn>().Property(r => r.ReturnAmount).HasPrecision(18, 2);

        builder.Entity<Warehouse>().HasIndex(w => w.WarehouseCode).IsUnique();

        builder.Entity<Warehouse>()
            .HasOne(w => w.Branch)
            .WithMany()
            .HasForeignKey(w => w.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Material>().HasIndex(m => m.MaterialCode).IsUnique();
        builder.Entity<Material>().Property(m => m.ReorderLevel).HasPrecision(18, 3);

        builder.Entity<Stock>().HasIndex(s => new { s.MaterialId, s.WarehouseId }).IsUnique();
        builder.Entity<Stock>().Property(s => s.QuantityOnHand).HasPrecision(18, 3);

        builder.Entity<Stock>()
            .HasOne(s => s.Material)
            .WithMany()
            .HasForeignKey(s => s.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Stock>()
            .HasOne(s => s.Warehouse)
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockTransfer>().Property(t => t.Quantity).HasPrecision(18, 3);

        builder.Entity<StockTransfer>()
            .HasOne(t => t.Material)
            .WithMany()
            .HasForeignKey(t => t.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockTransfer>()
            .HasOne(t => t.FromWarehouse)
            .WithMany()
            .HasForeignKey(t => t.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockTransfer>()
            .HasOne(t => t.ToWarehouse)
            .WithMany()
            .HasForeignKey(t => t.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockIssue>().Property(i => i.Quantity).HasPrecision(18, 3);

        builder.Entity<StockIssue>()
            .HasOne(i => i.Material)
            .WithMany()
            .HasForeignKey(i => i.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockIssue>()
            .HasOne(i => i.Warehouse)
            .WithMany()
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockReturn>().Property(r => r.Quantity).HasPrecision(18, 3);

        builder.Entity<StockReturn>()
            .HasOne(r => r.Material)
            .WithMany()
            .HasForeignKey(r => r.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockReturn>()
            .HasOne(r => r.Warehouse)
            .WithMany()
            .HasForeignKey(r => r.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockAdjustment>().Property(a => a.QuantityDelta).HasPrecision(18, 3);

        builder.Entity<StockAdjustment>()
            .HasOne(a => a.Material)
            .WithMany()
            .HasForeignKey(a => a.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockAdjustment>()
            .HasOne(a => a.Warehouse)
            .WithMany()
            .HasForeignKey(a => a.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Construction Project Management
        builder.Entity<WbsTask>().Property(t => t.PercentComplete).HasPrecision(5, 2);

        builder.Entity<WbsTask>()
            .HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WbsTask>()
            .HasOne(t => t.Parent)
            .WithMany()
            .HasForeignKey(t => t.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Milestone>()
            .HasOne(m => m.Project)
            .WithMany()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BoqItem>().Property(b => b.Quantity).HasPrecision(18, 3);
        builder.Entity<BoqItem>().Property(b => b.Rate).HasPrecision(18, 2);
        builder.Entity<BoqItem>().Property(b => b.Amount).HasPrecision(18, 2);

        builder.Entity<BoqItem>()
            .HasOne(b => b.Project)
            .WithMany()
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DailyProgress>().Property(d => d.PercentComplete).HasPrecision(5, 2);

        builder.Entity<DailyProgress>()
            .HasOne(d => d.Project)
            .WithMany()
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SitePhoto>()
            .HasOne(p => p.Project)
            .WithMany()
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SitePhoto>()
            .HasOne(p => p.DailyProgress)
            .WithMany()
            .HasForeignKey(p => p.DailyProgressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DelayEvent>()
            .HasOne(e => e.Project)
            .WithMany()
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DelayEvent>()
            .HasOne(e => e.WbsTask)
            .WithMany()
            .HasForeignKey(e => e.WbsTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BudgetLine>().Property(b => b.BudgetedAmount).HasPrecision(18, 2);
        builder.Entity<BudgetLine>().Property(b => b.ActualAmount).HasPrecision(18, 2);

        builder.Entity<BudgetLine>()
            .HasOne(b => b.Project)
            .WithMany()
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Engineering
        builder.Entity<Drawing>()
            .HasOne(d => d.Project)
            .WithMany()
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DrawingRevision>()
            .HasOne(r => r.Drawing)
            .WithMany()
            .HasForeignKey(r => r.DrawingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DrawingApproval>()
            .HasOne(a => a.Drawing)
            .WithMany()
            .HasForeignKey(a => a.DrawingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DrawingApproval>()
            .HasOne(a => a.DrawingRevision)
            .WithMany()
            .HasForeignKey(a => a.DrawingRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contractor Management
        builder.Entity<WorkOrder>().Property(w => w.Amount).HasPrecision(18, 2);

        builder.Entity<WorkOrder>()
            .HasOne(w => w.Contractor)
            .WithMany()
            .HasForeignKey(w => w.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WorkOrder>()
            .HasOne(w => w.Project)
            .WithMany()
            .HasForeignKey(w => w.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RateContract>().Property(r => r.Rate).HasPrecision(18, 2);

        builder.Entity<RateContract>()
            .HasOne(r => r.Contractor)
            .WithMany()
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RateContract>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RunningBill>().Property(b => b.WorkDoneAmount).HasPrecision(18, 2);
        builder.Entity<RunningBill>().Property(b => b.PreviousBillAmount).HasPrecision(18, 2);
        builder.Entity<RunningBill>().Property(b => b.DeductionAmount).HasPrecision(18, 2);
        builder.Entity<RunningBill>().Property(b => b.NetPayableAmount).HasPrecision(18, 2);

        builder.Entity<RunningBill>()
            .HasOne(b => b.WorkOrder)
            .WithMany()
            .HasForeignKey(b => b.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SecurityDeposit>().Property(s => s.DepositAmount).HasPrecision(18, 2);

        builder.Entity<SecurityDeposit>()
            .HasOne(s => s.Contractor)
            .WithMany()
            .HasForeignKey(s => s.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SecurityDeposit>()
            .HasOne(s => s.WorkOrder)
            .WithMany()
            .HasForeignKey(s => s.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PerformanceEvaluation>()
            .HasOne(p => p.Contractor)
            .WithMany()
            .HasForeignKey(p => p.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PerformanceEvaluation>()
            .HasOne(p => p.Project)
            .WithMany()
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ContractorLedger>().Property(l => l.DebitAmount).HasPrecision(18, 2);
        builder.Entity<ContractorLedger>().Property(l => l.CreditAmount).HasPrecision(18, 2);
        builder.Entity<ContractorLedger>().Property(l => l.Balance).HasPrecision(18, 2);

        builder.Entity<ContractorLedger>()
            .HasOne(l => l.Contractor)
            .WithMany()
            .HasForeignKey(l => l.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Labor Management
        builder.Entity<Worker>().Property(w => w.DailyWageRate).HasPrecision(18, 2);

        builder.Entity<Worker>()
            .HasOne(w => w.Contractor)
            .WithMany()
            .HasForeignKey(w => w.ContractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Attendance>().Property(a => a.HoursWorked).HasPrecision(5, 2);

        builder.Entity<Attendance>()
            .HasOne(a => a.Worker)
            .WithMany()
            .HasForeignKey(a => a.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Attendance>()
            .HasOne(a => a.Project)
            .WithMany()
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SafetyTraining>().Property(s => s.DurationHours).HasPrecision(5, 2);

        builder.Entity<SafetyTraining>()
            .HasOne(s => s.Worker)
            .WithMany()
            .HasForeignKey(s => s.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Overtime>().Property(o => o.Hours).HasPrecision(5, 2);
        builder.Entity<Overtime>().Property(o => o.RatePerHour).HasPrecision(18, 2);
        builder.Entity<Overtime>().Property(o => o.Amount).HasPrecision(18, 2);

        builder.Entity<Overtime>()
            .HasOne(o => o.Worker)
            .WithMany()
            .HasForeignKey(o => o.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Overtime>()
            .HasOne(o => o.Project)
            .WithMany()
            .HasForeignKey(o => o.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Salary>().Property(s => s.DaysWorked).HasPrecision(5, 2);
        builder.Entity<Salary>().Property(s => s.BasicAmount).HasPrecision(18, 2);
        builder.Entity<Salary>().Property(s => s.OvertimeAmount).HasPrecision(18, 2);
        builder.Entity<Salary>().Property(s => s.DeductionAmount).HasPrecision(18, 2);
        builder.Entity<Salary>().Property(s => s.NetAmount).HasPrecision(18, 2);

        builder.Entity<Salary>()
            .HasOne(s => s.Worker)
            .WithMany()
            .HasForeignKey(s => s.WorkerId)
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
        var softDeleteEntities = new[]
        {
            typeof(Company), typeof(Branch), typeof(Project), typeof(Department), typeof(CostCenter),
            typeof(Building), typeof(Tower), typeof(Floor), typeof(PropertyUnit),
            typeof(Lead), typeof(Inquiry), typeof(FollowUp), typeof(Customer),
            typeof(Quotation), typeof(Booking), typeof(SaleAgreement),
            typeof(InstallmentPlan), typeof(Installment), typeof(Receipt),
            typeof(Supplier), typeof(PurchaseRequisition), typeof(Rfq), typeof(VendorQuotation),
            typeof(PurchaseOrder), typeof(GoodsReceive), typeof(PurchaseReturn),
            typeof(Warehouse), typeof(Material), typeof(Stock), typeof(StockTransfer),
            typeof(StockIssue), typeof(StockReturn), typeof(StockAdjustment),
            typeof(WbsTask), typeof(Milestone), typeof(BoqItem), typeof(DailyProgress),
            typeof(SitePhoto), typeof(DelayEvent), typeof(BudgetLine),
            typeof(Drawing), typeof(DrawingRevision), typeof(DrawingApproval),
            typeof(Contractor), typeof(WorkOrder), typeof(RateContract), typeof(RunningBill),
            typeof(SecurityDeposit), typeof(PerformanceEvaluation), typeof(ContractorLedger),
            typeof(Worker), typeof(Attendance), typeof(SafetyTraining), typeof(Overtime), typeof(Salary)
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
