using Microsoft.AspNetCore.Identity;

namespace BuilderERP.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
}
