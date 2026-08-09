namespace BuilderERP.Domain.Entities;

public class Overtime : BaseEntity
{
    public DateTime OvertimeDate { get; set; } = DateTime.UtcNow;
    public decimal Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
}
