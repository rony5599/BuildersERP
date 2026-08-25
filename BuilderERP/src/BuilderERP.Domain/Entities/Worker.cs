namespace BuilderERP.Domain.Entities;

public class Worker : BaseEntity
{
    public string WorkerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Trade { get; set; }
    public decimal DailyWageRate { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public long? ContractorId { get; set; }
    public Contractor? Contractor { get; set; }
}
