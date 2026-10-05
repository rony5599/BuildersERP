using BuilderERP.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Employee> Employees => Set<Employee>();
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
    public DbSet<CustomerCommunication> CustomerCommunications => Set<CustomerCommunication>();
    public DbSet<SiteVisit> SiteVisits => Set<SiteVisit>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<SaleAgreement> SaleAgreements => Set<SaleAgreement>();
    public DbSet<Broker> Brokers => Set<Broker>();
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<InstallmentPlan> InstallmentPlans => Set<InstallmentPlan>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<CollectionTarget> CollectionTargets => Set<CollectionTarget>();
    public DbSet<PaymentReminder> PaymentReminders => Set<PaymentReminder>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseRequisition> PurchaseRequisitions => Set<PurchaseRequisition>();
    public DbSet<PurchaseRequisitionActionAssignment> PurchaseRequisitionActionAssignments => Set<PurchaseRequisitionActionAssignment>();
    public DbSet<CashRequisition> CashRequisitions => Set<CashRequisition>();
    public DbSet<CashRequisitionActionAssignment> CashRequisitionActionAssignments => Set<CashRequisitionActionAssignment>();
    public DbSet<CashRequisitionDetail> CashRequisitionDetails => Set<CashRequisitionDetail>();
    public DbSet<Rfq> Rfqs => Set<Rfq>();
    public DbSet<VendorQuotation> VendorQuotations => Set<VendorQuotation>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderActionAssignment> PurchaseOrderActionAssignments => Set<PurchaseOrderActionAssignment>();
    public DbSet<CashPurchaseOrder> CashPurchaseOrders => Set<CashPurchaseOrder>();
    public DbSet<CashPurchaseOrderActionAssignment> CashPurchaseOrderActionAssignments => Set<CashPurchaseOrderActionAssignment>();
    public DbSet<CashPurchaseOrderDetail> CashPurchaseOrderDetails => Set<CashPurchaseOrderDetail>();
    public DbSet<GoodsReceive> GoodsReceives => Set<GoodsReceive>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PoBill> PoBills => Set<PoBill>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<CashPoBill> CashPoBills => Set<CashPoBill>();
    public DbSet<CashDisbursement> CashDisbursements => Set<CashDisbursement>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockIssue> StockIssues => Set<StockIssue>();
    public DbSet<StockReturn> StockReturns => Set<StockReturn>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemPriceHistory> ItemPriceHistories => Set<ItemPriceHistory>();
    public DbSet<PurchaseRequisitionDetail> PurchaseRequisitionDetails => Set<PurchaseRequisitionDetail>();
    public DbSet<RfqVendor> RfqVendors => Set<RfqVendor>();
    public DbSet<RfqDetail> RfqDetails => Set<RfqDetail>();
    public DbSet<VendorQuotationDetail> VendorQuotationDetails => Set<VendorQuotationDetail>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    public DbSet<GoodsReceiveDetail> GoodsReceiveDetails => Set<GoodsReceiveDetail>();
    public DbSet<PurchaseReturnDetail> PurchaseReturnDetails => Set<PurchaseReturnDetail>();
    public DbSet<PoBillDetail> PoBillDetails => Set<PoBillDetail>();
    public DbSet<CashPoBillDetail> CashPoBillDetails => Set<CashPoBillDetail>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
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
    public DbSet<EngineerWorkOrderRequisition> EngineerWorkOrderRequisitions => Set<EngineerWorkOrderRequisition>();
    public DbSet<EngineerWorkOrderRequisitionActionAssignment> EngineerWorkOrderRequisitionActionAssignments => Set<EngineerWorkOrderRequisitionActionAssignment>();
    public DbSet<EngineerWorkOrderRequisitionDetail> EngineerWorkOrderRequisitionDetails => Set<EngineerWorkOrderRequisitionDetail>();
    public DbSet<EngineerWorkOrder> EngineerWorkOrders => Set<EngineerWorkOrder>();
    public DbSet<EngineerWorkOrderActionAssignment> EngineerWorkOrderActionAssignments => Set<EngineerWorkOrderActionAssignment>();
    public DbSet<EngineerWorkOrderDetail> EngineerWorkOrderDetails => Set<EngineerWorkOrderDetail>();
    public DbSet<EngineerWorkOrderPaymentHead> EngineerWorkOrderPaymentHeads => Set<EngineerWorkOrderPaymentHead>();
    public DbSet<EwoBill> EwoBills => Set<EwoBill>();
    public DbSet<EwoBillDetail> EwoBillDetails => Set<EwoBillDetail>();
    public DbSet<EwoBillHead> EwoBillHeads => Set<EwoBillHead>();
    public DbSet<EwoBillAdjustment> EwoBillAdjustments => Set<EwoBillAdjustment>();
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
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<EquipmentRental> EquipmentRentals => Set<EquipmentRental>();
    public DbSet<FuelLog> FuelLogs => Set<FuelLog>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<OperatorAssignment> OperatorAssignments => Set<OperatorAssignment>();
    public DbSet<MaterialInspection> MaterialInspections => Set<MaterialInspection>();
    public DbSet<SiteInspection> SiteInspections => Set<SiteInspection>();
    public DbSet<QualityChecklist> QualityChecklists => Set<QualityChecklist>();
    public DbSet<TestReport> TestReports => Set<TestReport>();
    public DbSet<Ncr> Ncrs => Set<Ncr>();
    public DbSet<PunchList> PunchLists => Set<PunchList>();
    public DbSet<PpeTracking> PpeTrackings => Set<PpeTracking>();
    public DbSet<SafetyInspection> SafetyInspections => Set<SafetyInspection>();
    public DbSet<SafetyAudit> SafetyAudits => Set<SafetyAudit>();
    public DbSet<IncidentReport> IncidentReports => Set<IncidentReport>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<LandDocument> LandDocuments => Set<LandDocument>();
    public DbSet<LandMutation> LandMutations => Set<LandMutation>();
    public DbSet<LandRegistration> LandRegistrations => Set<LandRegistration>();
    public DbSet<LegalCase> LegalCases => Set<LegalCase>();
    public DbSet<LegalAgreement> LegalAgreements => Set<LegalAgreement>();
    public DbSet<LegalNotice> LegalNotices => Set<LegalNotice>();
    public DbSet<FlatHandover> FlatHandovers => Set<FlatHandover>();
    public DbSet<SnagItem> SnagItems => Set<SnagItem>();
    public DbSet<DefectRecord> DefectRecords => Set<DefectRecord>();
    public DbSet<Warranty> Warranties => Set<Warranty>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();
    public DbSet<ApartmentMaintenance> ApartmentMaintenances => Set<ApartmentMaintenance>();
    public DbSet<UtilityBill> UtilityBills => Set<UtilityBill>();
    public DbSet<VisitorLog> VisitorLogs => Set<VisitorLog>();
    public DbSet<SecurityIncident> SecurityIncidents => Set<SecurityIncident>();
    public DbSet<ParkingSlot> ParkingSlots => Set<ParkingSlot>();
    public DbSet<CommonAreaBooking> CommonAreaBookings => Set<CommonAreaBooking>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetProperty("Guid")?.PropertyType == typeof(Guid))
            {
                builder.Entity(entityType.ClrType).HasIndex("Guid").IsUnique();
            }
        }

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

        builder.Entity<PurchaseOrderActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CashRequisitionActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CashPurchaseOrderActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PurchaseRequisitionActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<EngineerWorkOrderActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<EngineerWorkOrderRequisitionActionAssignment>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.Property(a => a.Action).HasConversion<string>().HasMaxLength(20);
            entity.Property(a => a.EntityName).HasMaxLength(200);
            entity.Property(a => a.ChangedBy).HasMaxLength(256);
            entity.Property(a => a.PropertyName).HasMaxLength(200);
            entity.HasIndex(a => new { a.EntityName, a.EntityId });
            entity.HasIndex(a => a.ChangedAt);
        });

        builder.Entity<UserDevice>(entity =>
        {
            entity.Property(d => d.DeviceId).HasMaxLength(100).IsRequired();
            entity.Property(d => d.DeviceName).HasMaxLength(200);
            entity.Property(d => d.DeviceType).HasMaxLength(50);
            entity.Property(d => d.Browser).HasMaxLength(100);
            entity.Property(d => d.OperatingSystem).HasMaxLength(100);
            entity.Property(d => d.IPAddress).HasMaxLength(45);
            entity.Property(d => d.LastIPAddress).HasMaxLength(45);
            entity.Property(d => d.UserAgent).HasMaxLength(512);

            entity.HasIndex(d => new { d.UserId, d.DeviceId }).IsUnique();
            entity.HasIndex(d => d.Status);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
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

        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Designation)
            .WithMany()
            .HasForeignKey(e => e.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Branch)
            .WithMany()
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.ReportingTo)
            .WithMany()
            .HasForeignKey(e => e.ReportingToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

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

        builder.Entity<CustomerCommunication>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SiteVisit>()
            .HasOne(v => v.Lead)
            .WithMany()
            .HasForeignKey(v => v.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SiteVisit>()
            .HasOne(v => v.PropertyUnit)
            .WithMany()
            .HasForeignKey(v => v.PropertyUnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<SiteVisit>()
            .HasOne(v => v.AssignedToUser)
            .WithMany()
            .HasForeignKey(v => v.AssignedToUserId)
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

        builder.Entity<Booking>()
            .HasOne(b => b.Broker)
            .WithMany()
            .HasForeignKey(b => b.BrokerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Booking>()
            .HasOne(b => b.CollectionOfficer)
            .WithMany()
            .HasForeignKey(b => b.CollectionOfficerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Booking>().Property(b => b.BookingAmount).HasPrecision(18, 2);

        builder.Entity<Commission>()
            .HasOne(c => c.Broker)
            .WithMany()
            .HasForeignKey(c => c.BrokerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Commission>()
            .HasOne(c => c.Booking)
            .WithMany()
            .HasForeignKey(c => c.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Commission>().Property(c => c.CommissionRate).HasPrecision(9, 4);
        builder.Entity<Commission>().Property(c => c.CommissionAmount).HasPrecision(18, 2);
        builder.Entity<Broker>().Property(b => b.DefaultCommissionRate).HasPrecision(9, 4);

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

        builder.Entity<CollectionTarget>()
            .HasOne(t => t.CollectionOfficer)
            .WithMany()
            .HasForeignKey(t => t.CollectionOfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CollectionTarget>()
            .HasIndex(t => new { t.CollectionOfficerId, t.Year, t.Month })
            .IsUnique();

        builder.Entity<CollectionTarget>().Property(t => t.TargetAmount).HasPrecision(18, 2);

        builder.Entity<PaymentReminder>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PaymentReminder>()
            .HasOne(r => r.Installment)
            .WithMany()
            .HasForeignKey(r => r.InstallmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Receipt>().Property(r => r.AmountPaid).HasPrecision(18, 2);

        builder.Entity<PurchaseRequisition>()
            .HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseRequisition>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseRequisition>().Property(r => r.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<Rfq>()
            .HasOne(q => q.PurchaseRequisition)
            .WithMany()
            .HasForeignKey(q => q.PurchaseRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RfqVendor>()
            .HasOne(v => v.Rfq)
            .WithMany(q => q.RfqVendors)
            .HasForeignKey(v => v.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RfqVendor>()
            .HasOne(v => v.Supplier)
            .WithMany()
            .HasForeignKey(v => v.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RfqVendor>().HasIndex(v => new { v.RfqId, v.SupplierId }).IsUnique();

        builder.Entity<RfqDetail>()
            .HasOne(d => d.Rfq)
            .WithMany(q => q.Details)
            .HasForeignKey(d => d.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RfqDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RfqDetail>().Property(d => d.Quantity).HasPrecision(18, 3);

        builder.Entity<PurchaseRequisitionDetail>()
            .HasOne(d => d.PurchaseRequisition)
            .WithMany(r => r.Details)
            .HasForeignKey(d => d.PurchaseRequisitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseRequisitionDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseRequisitionDetail>().Property(d => d.Quantity).HasPrecision(18, 3);
        builder.Entity<PurchaseRequisitionDetail>().Property(d => d.EstimatedUnitPrice).HasPrecision(18, 2);
        builder.Entity<PurchaseRequisitionDetail>().Property(d => d.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<CashRequisition>()
            .HasOne(r => r.RequesterEmployee)
            .WithMany()
            .HasForeignKey(r => r.RequesterEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashRequisition>()
            .HasOne(r => r.Department)
            .WithMany()
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashRequisition>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashRequisition>().Property(r => r.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<CashRequisitionDetail>()
            .HasOne(d => d.CashRequisition)
            .WithMany(r => r.Details)
            .HasForeignKey(d => d.CashRequisitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CashRequisitionDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashRequisitionDetail>().Property(d => d.Quantity).HasPrecision(18, 3);
        builder.Entity<CashRequisitionDetail>().Property(d => d.EstimatedUnitPrice).HasPrecision(18, 2);
        builder.Entity<CashRequisitionDetail>().Property(d => d.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<VendorQuotation>()
            .HasOne(v => v.Rfq)
            .WithMany(q => q.VendorQuotations)
            .HasForeignKey(v => v.RfqId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VendorQuotation>()
            .HasOne(v => v.Supplier)
            .WithMany()
            .HasForeignKey(v => v.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VendorQuotation>().Property(v => v.QuotedAmount).HasPrecision(18, 2);

        builder.Entity<VendorQuotationDetail>()
            .HasOne(d => d.VendorQuotation)
            .WithMany(v => v.Details)
            .HasForeignKey(d => d.VendorQuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<VendorQuotationDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VendorQuotationDetail>().Property(d => d.Quantity).HasPrecision(18, 3);
        builder.Entity<VendorQuotationDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<VendorQuotationDetail>().Property(d => d.NetAmount).HasPrecision(18, 2);

        builder.Entity<PurchaseOrder>()
            .HasOne(o => o.VendorQuotation)
            .WithMany()
            .HasForeignKey(o => o.VendorQuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrder>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrder>().Property(o => o.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<PurchaseOrderDetail>()
            .HasOne(d => d.PurchaseOrder)
            .WithMany(o => o.Details)
            .HasForeignKey(d => d.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseOrderDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrderDetail>().Property(d => d.OrderedQuantity).HasPrecision(18, 3);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.ReceivedQuantity).HasPrecision(18, 3);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseOrderDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<CashPurchaseOrder>()
            .HasOne(o => o.Supplier)
            .WithMany()
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPurchaseOrder>()
            .HasOne(o => o.CashRequisition)
            .WithMany()
            .HasForeignKey(o => o.CashRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPurchaseOrder>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Entity<CashPurchaseOrder>().Property(o => o.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<CashPurchaseOrderDetail>()
            .HasOne(d => d.CashPurchaseOrder)
            .WithMany(o => o.Details)
            .HasForeignKey(d => d.CashPurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CashPurchaseOrderDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.OrderedQuantity).HasPrecision(18, 3);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.ReceivedQuantity).HasPrecision(18, 3);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<CashPurchaseOrderDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<GoodsReceive>()
            .HasOne(g => g.PurchaseOrder)
            .WithMany(o => o.GoodsReceives)
            .HasForeignKey(g => g.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceive>()
            .HasOne(g => g.EngineerWorkOrder)
            .WithMany()
            .HasForeignKey(g => g.EngineerWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceive>()
            .HasOne(g => g.CashPurchaseOrder)
            .WithMany()
            .HasForeignKey(g => g.CashPurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceive>()
            .HasOne(g => g.Warehouse)
            .WithMany()
            .HasForeignKey(g => g.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceive>().Property(g => g.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<GoodsReceiveDetail>()
            .HasOne(d => d.GoodsReceive)
            .WithMany(g => g.Details)
            .HasForeignKey(d => d.GoodsReceiveId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GoodsReceiveDetail>()
            .HasOne(d => d.PurchaseOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.PurchaseOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceiveDetail>()
            .HasOne(d => d.EngineerWorkOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.EngineerWorkOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceiveDetail>()
            .HasOne(d => d.CashPurchaseOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.CashPurchaseOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceiveDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GoodsReceiveDetail>().Property(d => d.ReceivedQuantity).HasPrecision(18, 3);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<GoodsReceiveDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<PurchaseReturn>()
            .HasOne(r => r.GoodsReceive)
            .WithMany()
            .HasForeignKey(r => r.GoodsReceiveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseReturn>().Property(r => r.ReturnAmount).HasPrecision(18, 2);

        builder.Entity<DocumentSequence>().HasIndex(s => new { s.ProjectId, s.DocumentType }).IsUnique();

        builder.Entity<PurchaseReturnDetail>()
            .HasOne(d => d.PurchaseReturn)
            .WithMany(r => r.Details)
            .HasForeignKey(d => d.PurchaseReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseReturnDetail>()
            .HasOne(d => d.GoodsReceiveDetail)
            .WithMany()
            .HasForeignKey(d => d.GoodsReceiveDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseReturnDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseReturnDetail>().Property(d => d.ReturnQuantity).HasPrecision(18, 3);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<PurchaseReturnDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<PoBill>()
            .HasOne(b => b.PurchaseOrder)
            .WithMany()
            .HasForeignKey(b => b.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PoBill>().Property(b => b.TotalAmount).HasPrecision(18, 2);
        builder.Entity<PoBill>().HasIndex(b => b.BillNumber);

        builder.Entity<PoBillDetail>()
            .HasOne(d => d.PoBill)
            .WithMany(b => b.Details)
            .HasForeignKey(d => d.PoBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PoBillDetail>()
            .HasOne(d => d.PurchaseOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.PurchaseOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PoBillDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PoBillDetail>().Property(d => d.BilledQuantity).HasPrecision(18, 3);
        builder.Entity<PoBillDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<PoBillDetail>().Property(d => d.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<PoBillDetail>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Entity<PoBillDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<PoBillDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<PoBillDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<PoBillDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<PoBillDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<SupplierPayment>()
            .HasOne(p => p.PoBill)
            .WithMany()
            .HasForeignKey(p => p.PoBillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SupplierPayment>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SupplierPayment>()
            .HasOne(p => p.EwoBill)
            .WithMany()
            .HasForeignKey(p => p.EwoBillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SupplierPayment>().ToTable(t => t.HasCheckConstraint(
            "CK_SupplierPayments_OneBill",
            "([PoBillId] IS NOT NULL AND [EwoBillId] IS NULL) OR ([PoBillId] IS NULL AND [EwoBillId] IS NOT NULL)"));

        builder.Entity<SupplierPayment>().Property(p => p.Amount).HasPrecision(18, 2);
        builder.Entity<SupplierPayment>().HasIndex(p => p.PaymentNumber);
        builder.Entity<SupplierPayment>().HasIndex(p => p.SupplierId);

        builder.Entity<CashPoBill>()
            .HasOne(b => b.CashPurchaseOrder)
            .WithMany()
            .HasForeignKey(b => b.CashPurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPoBill>()
            .HasOne(b => b.RequesterEmployee)
            .WithMany()
            .HasForeignKey(b => b.RequesterEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPoBill>().Property(b => b.TotalAmount).HasPrecision(18, 2);
        builder.Entity<CashPoBill>().HasIndex(b => b.BillNumber);
        builder.Entity<CashPoBill>().HasIndex(b => b.RequesterEmployeeId);

        builder.Entity<CashPoBillDetail>()
            .HasOne(d => d.CashPoBill)
            .WithMany(b => b.Details)
            .HasForeignKey(d => d.CashPoBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CashPoBillDetail>()
            .HasOne(d => d.CashPurchaseOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.CashPurchaseOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPoBillDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashPoBillDetail>().Property(d => d.BilledQuantity).HasPrecision(18, 3);
        builder.Entity<CashPoBillDetail>().Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.VatPercent).HasPrecision(5, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.VatAmount).HasPrecision(18, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.TaxPercent).HasPrecision(5, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Entity<CashPoBillDetail>().Property(d => d.LineTotal).HasPrecision(18, 2);

        builder.Entity<CashDisbursement>()
            .HasOne(d => d.CashRequisition)
            .WithMany()
            .HasForeignKey(d => d.CashRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashDisbursement>()
            .HasOne(d => d.RequesterEmployee)
            .WithMany()
            .HasForeignKey(d => d.RequesterEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CashDisbursement>().Property(d => d.Amount).HasPrecision(18, 2);
        builder.Entity<CashDisbursement>().HasIndex(d => d.DisbursementNumber);
        builder.Entity<CashDisbursement>().HasIndex(d => d.RequesterEmployeeId);

        builder.Entity<ItemCategory>().HasIndex(c => c.Code).IsUnique();

        builder.Entity<ItemCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany()
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Material>()
            .HasOne(m => m.Category)
            .WithMany()
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Material>().Property(m => m.UnitConversionFactor).HasPrecision(18, 4);
        builder.Entity<Material>().Property(m => m.MinStockLevel).HasPrecision(18, 3);
        builder.Entity<Material>().Property(m => m.MaxStockLevel).HasPrecision(18, 3);
        builder.Entity<Material>().Property(m => m.AveragePurchasePrice).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.LastPurchasePrice).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.StandardPurchasePrice).HasPrecision(18, 2);
        builder.Entity<Material>().Property(m => m.VatPercent).HasPrecision(5, 2);
        builder.Entity<Material>().Property(m => m.TaxPercent).HasPrecision(5, 2);
        builder.Entity<Material>().Property(m => m.DiscountPercent).HasPrecision(5, 2);

        builder.Entity<Supplier>().Property(s => s.CreditLimit).HasPrecision(18, 2);
        builder.Entity<Supplier>().Property(s => s.Rating).HasPrecision(3, 1);

        builder.Entity<ItemPriceHistory>()
            .HasOne(h => h.Material)
            .WithMany()
            .HasForeignKey(h => h.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ItemPriceHistory>()
            .HasOne(h => h.Supplier)
            .WithMany()
            .HasForeignKey(h => h.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ItemPriceHistory>().Property(h => h.UnitPrice).HasPrecision(18, 2);
        builder.Entity<ItemPriceHistory>().Property(h => h.VatPercent).HasPrecision(5, 2);
        builder.Entity<ItemPriceHistory>().Property(h => h.TaxPercent).HasPrecision(5, 2);
        builder.Entity<ItemPriceHistory>().Property(h => h.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<ItemPriceHistory>().Property(h => h.NetPrice).HasPrecision(18, 2);
        builder.Entity<ItemPriceHistory>().Property(h => h.MinimumOrderQuantity).HasPrecision(18, 3);

        builder.Entity<InventoryTransaction>()
            .HasOne(t => t.Material)
            .WithMany()
            .HasForeignKey(t => t.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryTransaction>()
            .HasOne(t => t.Warehouse)
            .WithMany()
            .HasForeignKey(t => t.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryTransaction>().Property(t => t.QuantityIn).HasPrecision(18, 3);
        builder.Entity<InventoryTransaction>().Property(t => t.QuantityOut).HasPrecision(18, 3);
        builder.Entity<InventoryTransaction>().Property(t => t.BalanceQuantity).HasPrecision(18, 3);
        builder.Entity<InventoryTransaction>().Property(t => t.UnitCost).HasPrecision(18, 2);
        builder.Entity<InventoryTransaction>().Property(t => t.TotalCost).HasPrecision(18, 2);

        builder.Entity<Warehouse>().HasIndex(w => w.WarehouseCode).IsUnique();

        builder.Entity<Warehouse>()
            .HasOne(w => w.Branch)
            .WithMany()
            .HasForeignKey(w => w.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Warehouse>()
            .HasOne(w => w.Project)
            .WithMany()
            .HasForeignKey(w => w.ProjectId)
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
        builder.Entity<StockIssue>().Property(i => i.ConsumedQuantity).HasPrecision(18, 3);
        builder.Entity<StockIssue>().Property(i => i.WastageQuantity).HasPrecision(18, 3);

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

        builder.Entity<WbsTask>()
            .HasOne(t => t.PropertyUnit)
            .WithMany()
            .HasForeignKey(t => t.PropertyUnitId)
            .OnDelete(DeleteBehavior.SetNull);

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

        builder.Entity<EngineerWorkOrderRequisition>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrderRequisition>().Property(r => r.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<EngineerWorkOrderRequisitionDetail>()
            .HasOne(d => d.EngineerWorkOrderRequisition)
            .WithMany(r => r.Details)
            .HasForeignKey(d => d.EngineerWorkOrderRequisitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EngineerWorkOrderRequisitionDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrderRequisitionDetail>().Property(d => d.Quantity).HasPrecision(18, 3);
        builder.Entity<EngineerWorkOrderRequisitionDetail>().Property(d => d.EstimatedUnitPrice).HasPrecision(18, 2);
        builder.Entity<EngineerWorkOrderRequisitionDetail>().Property(d => d.EstimatedAmount).HasPrecision(18, 2);

        builder.Entity<EngineerWorkOrder>()
            .HasOne(w => w.EngineerWorkOrderRequisition)
            .WithMany()
            .HasForeignKey(w => w.EngineerWorkOrderRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrder>()
            .HasOne(w => w.Supplier)
            .WithMany()
            .HasForeignKey(w => w.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrder>()
            .HasOne(w => w.MotherWorkOrder)
            .WithMany(w => w.Revisions)
            .HasForeignKey(w => w.MotherWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrder>()
            .HasOne(w => w.PreviousWorkOrder)
            .WithMany()
            .HasForeignKey(w => w.PreviousWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrder>().Property(w => w.TotalAmount).HasPrecision(18, 2);

        builder.Entity<EngineerWorkOrderDetail>()
            .HasOne(d => d.EngineerWorkOrder)
            .WithMany(w => w.Details)
            .HasForeignKey(d => d.EngineerWorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EngineerWorkOrderDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EngineerWorkOrderDetail>().Property(d => d.Qty).HasPrecision(18, 3);
        builder.Entity<EngineerWorkOrderDetail>().Property(d => d.Rate).HasPrecision(18, 2);
        builder.Entity<EngineerWorkOrderDetail>().Property(d => d.Amount).HasPrecision(18, 2);
        builder.Entity<EngineerWorkOrderDetail>().Property(d => d.ReceivedQuantity).HasPrecision(18, 3);

        builder.Entity<EngineerWorkOrder>().Property(w => w.ReceivedAmount).HasPrecision(18, 2);

        builder.Entity<EngineerWorkOrderPaymentHead>()
            .HasOne(h => h.EngineerWorkOrder)
            .WithMany(w => w.PaymentHeads)
            .HasForeignKey(h => h.EngineerWorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EngineerWorkOrderPaymentHead>().Property(h => h.HeadName).HasMaxLength(100);
        builder.Entity<EngineerWorkOrderPaymentHead>().Property(h => h.Percent).HasPrecision(5, 2);

        builder.Entity<EwoBill>()
            .HasOne(b => b.EngineerWorkOrder)
            .WithMany()
            .HasForeignKey(b => b.EngineerWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBill>()
            .HasOne(b => b.RootWorkOrder)
            .WithMany()
            .HasForeignKey(b => b.RootWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBill>()
            .HasOne(b => b.Supplier)
            .WithMany()
            .HasForeignKey(b => b.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBill>().Property(b => b.MeasuredAmount).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.CumulativePercent).HasPrecision(5, 2);
        builder.Entity<EwoBill>().Property(b => b.CumulativeDue).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.PreviouslyCertified).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.CertifiedAmount).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.AdditionAmount).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.DeductionAmount).HasPrecision(18, 2);
        builder.Entity<EwoBill>().Property(b => b.NetPayable).HasPrecision(18, 2);
        builder.Entity<EwoBill>().HasIndex(b => b.BillNumber);

        builder.Entity<EwoBillDetail>()
            .HasOne(d => d.EwoBill)
            .WithMany(b => b.Details)
            .HasForeignKey(d => d.EwoBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EwoBillDetail>()
            .HasOne(d => d.EngineerWorkOrderDetail)
            .WithMany()
            .HasForeignKey(d => d.EngineerWorkOrderDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBillDetail>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBillDetail>().Property(d => d.MeasuredQuantity).HasPrecision(18, 3);
        builder.Entity<EwoBillDetail>().Property(d => d.Rate).HasPrecision(18, 2);
        builder.Entity<EwoBillDetail>().Property(d => d.Amount).HasPrecision(18, 2);

        builder.Entity<EwoBillHead>()
            .HasOne(h => h.EwoBill)
            .WithMany(b => b.Heads)
            .HasForeignKey(h => h.EwoBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EwoBillHead>()
            .HasOne(h => h.EngineerWorkOrderPaymentHead)
            .WithMany()
            .HasForeignKey(h => h.EngineerWorkOrderPaymentHeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EwoBillHead>().Property(h => h.HeadName).HasMaxLength(100);
        builder.Entity<EwoBillHead>().Property(h => h.HeadPercent).HasPrecision(5, 2);
        builder.Entity<EwoBillHead>().Property(h => h.ClaimPercent).HasPrecision(5, 2);

        builder.Entity<EwoBillAdjustment>()
            .HasOne(a => a.EwoBill)
            .WithMany(b => b.Adjustments)
            .HasForeignKey(a => a.EwoBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EwoBillAdjustment>().Property(a => a.Description).HasMaxLength(200);
        builder.Entity<EwoBillAdjustment>().Property(a => a.Amount).HasPrecision(18, 2);

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

        // Equipment Management
        builder.Entity<EquipmentRental>().Property(r => r.RatePerDay).HasPrecision(18, 2);
        builder.Entity<EquipmentRental>().Property(r => r.TotalAmount).HasPrecision(18, 2);

        builder.Entity<EquipmentRental>()
            .HasOne(r => r.Equipment)
            .WithMany()
            .HasForeignKey(r => r.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentRental>()
            .HasOne(r => r.Supplier)
            .WithMany()
            .HasForeignKey(r => r.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EquipmentRental>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FuelLog>().Property(f => f.FuelQuantity).HasPrecision(18, 3);
        builder.Entity<FuelLog>().Property(f => f.FuelCost).HasPrecision(18, 2);
        builder.Entity<FuelLog>().Property(f => f.MeterReading).HasPrecision(18, 2);

        builder.Entity<FuelLog>()
            .HasOne(f => f.Equipment)
            .WithMany()
            .HasForeignKey(f => f.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaintenanceRecord>().Property(m => m.Cost).HasPrecision(18, 2);

        builder.Entity<MaintenanceRecord>()
            .HasOne(m => m.Equipment)
            .WithMany()
            .HasForeignKey(m => m.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OperatorAssignment>()
            .HasOne(o => o.Equipment)
            .WithMany()
            .HasForeignKey(o => o.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OperatorAssignment>()
            .HasOne(o => o.Worker)
            .WithMany()
            .HasForeignKey(o => o.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OperatorAssignment>()
            .HasOne(o => o.Project)
            .WithMany()
            .HasForeignKey(o => o.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quality Control
        builder.Entity<MaterialInspection>().Property(m => m.Quantity).HasPrecision(18, 3);

        builder.Entity<MaterialInspection>()
            .HasOne(m => m.Project)
            .WithMany()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaterialInspection>()
            .HasOne(m => m.Material)
            .WithMany()
            .HasForeignKey(m => m.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SiteInspection>()
            .HasOne(s => s.Project)
            .WithMany()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<QualityChecklist>()
            .HasOne(q => q.Project)
            .WithMany()
            .HasForeignKey(q => q.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestReport>()
            .HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TestReport>()
            .HasOne(t => t.Material)
            .WithMany()
            .HasForeignKey(t => t.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ncr>()
            .HasOne(n => n.Project)
            .WithMany()
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PunchList>()
            .HasOne(p => p.Project)
            .WithMany()
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Safety (HSE)
        builder.Entity<PpeTracking>()
            .HasOne(p => p.Worker)
            .WithMany()
            .HasForeignKey(p => p.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SafetyInspection>()
            .HasOne(s => s.Project)
            .WithMany()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SafetyAudit>().Property(a => a.Score).HasPrecision(5, 2);

        builder.Entity<SafetyAudit>()
            .HasOne(a => a.Project)
            .WithMany()
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<IncidentReport>()
            .HasOne(i => i.Project)
            .WithMany()
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RiskAssessment>()
            .HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Document Management
        builder.Entity<Document>()
            .HasOne(d => d.Customer)
            .WithMany()
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Document>()
            .HasOne(d => d.Project)
            .WithMany()
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DocumentVersion>()
            .HasOne(v => v.Document)
            .WithMany()
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Legal Module, After Handover, Facility Management
        builder.Entity<LandDocument>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LandMutation>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LandRegistration>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LegalCase>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LegalAgreement>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LegalNotice>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FlatHandover>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FlatHandover>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SnagItem>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DefectRecord>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Warranty>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MaintenanceRequest>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ServiceTicket>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApartmentMaintenance>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UtilityBill>()
            .HasOne(x => x.PropertyUnit)
            .WithMany()
            .HasForeignKey(x => x.PropertyUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VisitorLog>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SecurityIncident>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ParkingSlot>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CommonAreaBooking>()
            .HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
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
            typeof(Lead), typeof(Inquiry), typeof(FollowUp), typeof(Customer), typeof(CustomerCommunication), typeof(SiteVisit),
            typeof(Quotation), typeof(Booking), typeof(SaleAgreement), typeof(Broker), typeof(Commission),
            typeof(InstallmentPlan), typeof(Installment), typeof(Receipt),
            typeof(CollectionTarget), typeof(PaymentReminder),
            typeof(Supplier), typeof(PurchaseRequisition), typeof(CashRequisition), typeof(CashRequisitionDetail), typeof(Rfq), typeof(VendorQuotation),
            typeof(PurchaseOrder), typeof(CashPurchaseOrder), typeof(CashPurchaseOrderDetail), typeof(GoodsReceive), typeof(PurchaseReturn), typeof(PoBill), typeof(SupplierPayment), typeof(CashPoBill), typeof(CashDisbursement),
            typeof(Warehouse), typeof(Material), typeof(Stock), typeof(StockTransfer),
            typeof(StockIssue), typeof(StockReturn), typeof(StockAdjustment),
            typeof(ItemCategory), typeof(ItemPriceHistory), typeof(PurchaseRequisitionDetail),
            typeof(RfqVendor), typeof(RfqDetail), typeof(VendorQuotationDetail),
            typeof(PurchaseOrderDetail), typeof(GoodsReceiveDetail), typeof(PurchaseReturnDetail), typeof(PoBillDetail), typeof(CashPoBillDetail),
            typeof(InventoryTransaction),
            typeof(WbsTask), typeof(Milestone), typeof(BoqItem), typeof(DailyProgress),
            typeof(SitePhoto), typeof(DelayEvent), typeof(BudgetLine),
            typeof(Drawing), typeof(DrawingRevision), typeof(DrawingApproval),
            typeof(Contractor), typeof(WorkOrder), typeof(RateContract), typeof(RunningBill),
            typeof(EngineerWorkOrderRequisition), typeof(EngineerWorkOrderRequisitionDetail),
            typeof(EngineerWorkOrder), typeof(EngineerWorkOrderDetail), typeof(EngineerWorkOrderPaymentHead),
            typeof(EwoBill), typeof(EwoBillDetail), typeof(EwoBillHead), typeof(EwoBillAdjustment),
            typeof(SecurityDeposit), typeof(PerformanceEvaluation), typeof(ContractorLedger),
            typeof(Worker), typeof(Attendance), typeof(SafetyTraining), typeof(Overtime), typeof(Salary),
            typeof(Equipment), typeof(EquipmentRental), typeof(FuelLog), typeof(MaintenanceRecord), typeof(OperatorAssignment),
            typeof(MaterialInspection), typeof(SiteInspection), typeof(QualityChecklist),
            typeof(TestReport), typeof(Ncr), typeof(PunchList),
            typeof(PpeTracking), typeof(SafetyInspection), typeof(SafetyAudit),
            typeof(IncidentReport), typeof(RiskAssessment),
            typeof(Document), typeof(DocumentVersion),
            typeof(LandDocument), typeof(LandMutation), typeof(LandRegistration),
            typeof(LegalCase), typeof(LegalAgreement), typeof(LegalNotice),
            typeof(FlatHandover), typeof(SnagItem), typeof(DefectRecord), typeof(Warranty),
            typeof(MaintenanceRequest), typeof(ServiceTicket),
            typeof(ApartmentMaintenance), typeof(UtilityBill), typeof(VisitorLog),
            typeof(SecurityIncident), typeof(ParkingSlot), typeof(CommonAreaBooking)
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
