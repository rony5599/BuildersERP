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
    };
}
