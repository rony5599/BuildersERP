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
    };
}
