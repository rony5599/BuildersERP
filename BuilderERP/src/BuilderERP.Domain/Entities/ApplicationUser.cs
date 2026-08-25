using Microsoft.AspNetCore.Identity;

namespace BuilderERP.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public long? CompanyId { get; set; }
    public Company? Company { get; set; }

    public long? BranchId { get; set; }
    public Branch? Branch { get; set; }
}
