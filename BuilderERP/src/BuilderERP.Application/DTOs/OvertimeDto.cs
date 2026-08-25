namespace BuilderERP.Application.DTOs;

public class OvertimeDto
{
    public long Id { get; set; }
    public DateTime OvertimeDate { get; set; }
    public decimal Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public long WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
}

public class CreateOvertimeDto
{
    public DateTime OvertimeDate { get; set; } = DateTime.UtcNow;
    public decimal Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public long WorkerId { get; set; }
    public long? ProjectId { get; set; }
}

public class UpdateOvertimeDto
{
    public long Id { get; set; }
    public DateTime OvertimeDate { get; set; }
    public decimal Hours { get; set; }
    public decimal RatePerHour { get; set; }
    public long WorkerId { get; set; }
    public long? ProjectId { get; set; }
}
