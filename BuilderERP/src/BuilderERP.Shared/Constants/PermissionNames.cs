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

    public static readonly IReadOnlyList<(string Name, string Module)> All = new[]
    {
        (CompanyView, "Company"), (CompanyManage, "Company"),
        (BranchView, "Branch"), (BranchManage, "Branch"),
        (DepartmentView, "Department"), (DepartmentManage, "Department"),
        (ProjectView, "Project"), (ProjectManage, "Project"),
        (CostCenterView, "CostCenter"), (CostCenterManage, "CostCenter"),
        (UserView, "User"), (UserManage, "User"),
        (RoleView, "Role"), (RoleManage, "Role"),
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
    };
}
