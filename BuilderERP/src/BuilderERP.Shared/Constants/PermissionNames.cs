namespace BuilderERP.Shared.Constants;

public static class PermissionNames
{
    public const string CompanyView = "Company.View";
    public const string CompanyManage = "Company.Manage";

    public const string BranchView = "Branch.View";
    public const string BranchManage = "Branch.Manage";

    public const string DepartmentView = "Department.View";
    public const string DepartmentManage = "Department.Manage";

    public const string ProjectView = "Project.View";
    public const string ProjectManage = "Project.Manage";

    public const string CostCenterView = "CostCenter.View";
    public const string CostCenterManage = "CostCenter.Manage";

    public const string UserView = "User.View";
    public const string UserManage = "User.Manage";

    public const string RoleView = "Role.View";
    public const string RoleManage = "Role.Manage";

    public const string BuildingView = "Building.View";
    public const string BuildingManage = "Building.Manage";

    public const string TowerView = "Tower.View";
    public const string TowerManage = "Tower.Manage";

    public const string FloorView = "Floor.View";
    public const string FloorManage = "Floor.Manage";

    public const string PropertyUnitView = "PropertyUnit.View";
    public const string PropertyUnitManage = "PropertyUnit.Manage";

    public const string LeadView = "Lead.View";
    public const string LeadManage = "Lead.Manage";

    public const string InquiryView = "Inquiry.View";
    public const string InquiryManage = "Inquiry.Manage";

    public const string FollowUpView = "FollowUp.View";
    public const string FollowUpManage = "FollowUp.Manage";

    public const string CustomerView = "Customer.View";
    public const string CustomerManage = "Customer.Manage";

    public const string QuotationView = "Quotation.View";
    public const string QuotationManage = "Quotation.Manage";

    public const string BookingView = "Booking.View";
    public const string BookingManage = "Booking.Manage";

    public const string SaleAgreementView = "SaleAgreement.View";
    public const string SaleAgreementManage = "SaleAgreement.Manage";

    public const string InstallmentPlanView = "InstallmentPlan.View";
    public const string InstallmentPlanManage = "InstallmentPlan.Manage";

    public const string InstallmentView = "Installment.View";
    public const string InstallmentManage = "Installment.Manage";

    public const string ReceiptView = "Receipt.View";
    public const string ReceiptManage = "Receipt.Manage";

    public const string SupplierView = "Supplier.View";
    public const string SupplierManage = "Supplier.Manage";

    public const string PurchaseRequisitionView = "PurchaseRequisition.View";
    public const string PurchaseRequisitionManage = "PurchaseRequisition.Manage";

    public const string RfqView = "Rfq.View";
    public const string RfqManage = "Rfq.Manage";

    public const string VendorQuotationView = "VendorQuotation.View";
    public const string VendorQuotationManage = "VendorQuotation.Manage";

    public const string PurchaseOrderView = "PurchaseOrder.View";
    public const string PurchaseOrderManage = "PurchaseOrder.Manage";

    public const string GoodsReceiveView = "GoodsReceive.View";
    public const string GoodsReceiveManage = "GoodsReceive.Manage";

    public const string PurchaseReturnView = "PurchaseReturn.View";
    public const string PurchaseReturnManage = "PurchaseReturn.Manage";

    public const string WarehouseView = "Warehouse.View";
    public const string WarehouseManage = "Warehouse.Manage";

    public const string MaterialView = "Material.View";
    public const string MaterialManage = "Material.Manage";

    public const string StockView = "Stock.View";
    public const string StockManage = "Stock.Manage";

    public const string StockTransferView = "StockTransfer.View";
    public const string StockTransferManage = "StockTransfer.Manage";

    public const string StockIssueView = "StockIssue.View";
    public const string StockIssueManage = "StockIssue.Manage";

    public const string StockReturnView = "StockReturn.View";
    public const string StockReturnManage = "StockReturn.Manage";

    public const string StockAdjustmentView = "StockAdjustment.View";
    public const string StockAdjustmentManage = "StockAdjustment.Manage";

    public const string WbsTaskView = "WbsTask.View";
    public const string WbsTaskManage = "WbsTask.Manage";

    public const string MilestoneView = "Milestone.View";
    public const string MilestoneManage = "Milestone.Manage";

    public const string BoqItemView = "BoqItem.View";
    public const string BoqItemManage = "BoqItem.Manage";

    public const string DailyProgressView = "DailyProgress.View";
    public const string DailyProgressManage = "DailyProgress.Manage";

    public const string SitePhotoView = "SitePhoto.View";
    public const string SitePhotoManage = "SitePhoto.Manage";

    public const string DelayEventView = "DelayEvent.View";
    public const string DelayEventManage = "DelayEvent.Manage";

    public const string BudgetLineView = "BudgetLine.View";
    public const string BudgetLineManage = "BudgetLine.Manage";

    public const string DrawingView = "Drawing.View";
    public const string DrawingManage = "Drawing.Manage";

    public const string DrawingRevisionView = "DrawingRevision.View";
    public const string DrawingRevisionManage = "DrawingRevision.Manage";

    public const string DrawingApprovalView = "DrawingApproval.View";
    public const string DrawingApprovalManage = "DrawingApproval.Manage";

    public const string ContractorView = "Contractor.View";
    public const string ContractorManage = "Contractor.Manage";

    public const string WorkOrderView = "WorkOrder.View";
    public const string WorkOrderManage = "WorkOrder.Manage";

    public const string RateContractView = "RateContract.View";
    public const string RateContractManage = "RateContract.Manage";

    public const string RunningBillView = "RunningBill.View";
    public const string RunningBillManage = "RunningBill.Manage";

    public const string SecurityDepositView = "SecurityDeposit.View";
    public const string SecurityDepositManage = "SecurityDeposit.Manage";

    public const string PerformanceEvaluationView = "PerformanceEvaluation.View";
    public const string PerformanceEvaluationManage = "PerformanceEvaluation.Manage";

    public const string ContractorLedgerView = "ContractorLedger.View";
    public const string ContractorLedgerManage = "ContractorLedger.Manage";

    public static readonly IReadOnlyList<(string Name, string Module)> All = new[]
    {
        (CompanyView, "Company"), (CompanyManage, "Company"),
        (BranchView, "Branch"), (BranchManage, "Branch"),
        (DepartmentView, "Department"), (DepartmentManage, "Department"),
        (ProjectView, "Project"), (ProjectManage, "Project"),
        (CostCenterView, "CostCenter"), (CostCenterManage, "CostCenter"),
        (UserView, "User"), (UserManage, "User"),
        (RoleView, "Role"), (RoleManage, "Role"),
        (BuildingView, "Building"), (BuildingManage, "Building"),
        (TowerView, "Tower"), (TowerManage, "Tower"),
        (FloorView, "Floor"), (FloorManage, "Floor"),
        (PropertyUnitView, "PropertyUnit"), (PropertyUnitManage, "PropertyUnit"),
        (LeadView, "Lead"), (LeadManage, "Lead"),
        (InquiryView, "Inquiry"), (InquiryManage, "Inquiry"),
        (FollowUpView, "FollowUp"), (FollowUpManage, "FollowUp"),
        (CustomerView, "Customer"), (CustomerManage, "Customer"),
        (QuotationView, "Quotation"), (QuotationManage, "Quotation"),
        (BookingView, "Booking"), (BookingManage, "Booking"),
        (SaleAgreementView, "SaleAgreement"), (SaleAgreementManage, "SaleAgreement"),
        (InstallmentPlanView, "InstallmentPlan"), (InstallmentPlanManage, "InstallmentPlan"),
        (InstallmentView, "Installment"), (InstallmentManage, "Installment"),
        (ReceiptView, "Receipt"), (ReceiptManage, "Receipt"),
        (SupplierView, "Supplier"), (SupplierManage, "Supplier"),
        (PurchaseRequisitionView, "PurchaseRequisition"), (PurchaseRequisitionManage, "PurchaseRequisition"),
        (RfqView, "Rfq"), (RfqManage, "Rfq"),
        (VendorQuotationView, "VendorQuotation"), (VendorQuotationManage, "VendorQuotation"),
        (PurchaseOrderView, "PurchaseOrder"), (PurchaseOrderManage, "PurchaseOrder"),
        (GoodsReceiveView, "GoodsReceive"), (GoodsReceiveManage, "GoodsReceive"),
        (PurchaseReturnView, "PurchaseReturn"), (PurchaseReturnManage, "PurchaseReturn"),
        (WarehouseView, "Warehouse"), (WarehouseManage, "Warehouse"),
        (MaterialView, "Material"), (MaterialManage, "Material"),
        (StockView, "Stock"), (StockManage, "Stock"),
        (StockTransferView, "StockTransfer"), (StockTransferManage, "StockTransfer"),
        (StockIssueView, "StockIssue"), (StockIssueManage, "StockIssue"),
        (StockReturnView, "StockReturn"), (StockReturnManage, "StockReturn"),
        (StockAdjustmentView, "StockAdjustment"), (StockAdjustmentManage, "StockAdjustment"),
        (WbsTaskView, "WbsTask"), (WbsTaskManage, "WbsTask"),
        (MilestoneView, "Milestone"), (MilestoneManage, "Milestone"),
        (BoqItemView, "BoqItem"), (BoqItemManage, "BoqItem"),
        (DailyProgressView, "DailyProgress"), (DailyProgressManage, "DailyProgress"),
        (SitePhotoView, "SitePhoto"), (SitePhotoManage, "SitePhoto"),
        (DelayEventView, "DelayEvent"), (DelayEventManage, "DelayEvent"),
        (BudgetLineView, "BudgetLine"), (BudgetLineManage, "BudgetLine"),
        (DrawingView, "Drawing"), (DrawingManage, "Drawing"),
        (DrawingRevisionView, "DrawingRevision"), (DrawingRevisionManage, "DrawingRevision"),
        (DrawingApprovalView, "DrawingApproval"), (DrawingApprovalManage, "DrawingApproval"),
        (ContractorView, "Contractor"), (ContractorManage, "Contractor"),
        (WorkOrderView, "WorkOrder"), (WorkOrderManage, "WorkOrder"),
        (RateContractView, "RateContract"), (RateContractManage, "RateContract"),
        (RunningBillView, "RunningBill"), (RunningBillManage, "RunningBill"),
        (SecurityDepositView, "SecurityDeposit"), (SecurityDepositManage, "SecurityDeposit"),
        (PerformanceEvaluationView, "PerformanceEvaluation"), (PerformanceEvaluationManage, "PerformanceEvaluation"),
        (ContractorLedgerView, "ContractorLedger"), (ContractorLedgerManage, "ContractorLedger"),
    };

    public static readonly IReadOnlyList<string> AdminGrants = new[]
    {
        CompanyView, CompanyManage,
        BranchView, BranchManage,
        DepartmentView, DepartmentManage,
        ProjectView, ProjectManage,
        CostCenterView, CostCenterManage,
        UserView, UserManage,
        RoleView,
        BuildingView, BuildingManage,
        TowerView, TowerManage,
        FloorView, FloorManage,
        PropertyUnitView, PropertyUnitManage,
        LeadView, LeadManage,
        InquiryView, InquiryManage,
        FollowUpView, FollowUpManage,
        CustomerView, CustomerManage,
        QuotationView, QuotationManage,
        BookingView, BookingManage,
        SaleAgreementView, SaleAgreementManage,
        InstallmentPlanView, InstallmentPlanManage,
        InstallmentView, InstallmentManage,
        ReceiptView, ReceiptManage,
        SupplierView, SupplierManage,
        PurchaseRequisitionView, PurchaseRequisitionManage,
        RfqView, RfqManage,
        VendorQuotationView, VendorQuotationManage,
        PurchaseOrderView, PurchaseOrderManage,
        GoodsReceiveView, GoodsReceiveManage,
        PurchaseReturnView, PurchaseReturnManage,
        WarehouseView, WarehouseManage,
        MaterialView, MaterialManage,
        StockView, StockManage,
        StockTransferView, StockTransferManage,
        StockIssueView, StockIssueManage,
        StockReturnView, StockReturnManage,
        StockAdjustmentView, StockAdjustmentManage,
        WbsTaskView, WbsTaskManage,
        MilestoneView, MilestoneManage,
        BoqItemView, BoqItemManage,
        DailyProgressView, DailyProgressManage,
        SitePhotoView, SitePhotoManage,
        DelayEventView, DelayEventManage,
        BudgetLineView, BudgetLineManage,
        DrawingView, DrawingManage,
        DrawingRevisionView, DrawingRevisionManage,
        DrawingApprovalView, DrawingApprovalManage,
        ContractorView, ContractorManage,
        WorkOrderView, WorkOrderManage,
        RateContractView, RateContractManage,
        RunningBillView, RunningBillManage,
        SecurityDepositView, SecurityDepositManage,
        PerformanceEvaluationView, PerformanceEvaluationManage,
        ContractorLedgerView, ContractorLedgerManage,
    };
}
