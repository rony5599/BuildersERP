namespace BuilderERP.Application.DTOs;

public class WorkerDto
{
    public Guid Id { get; set; }
    public string WorkerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Trade { get; set; }
    public decimal DailyWageRate { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? ContractorId { get; set; }
    public string? ContractorName { get; set; }
}

public class CreateWorkerDto
{
    public string WorkerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Trade { get; set; }
    public decimal DailyWageRate { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public Guid? ContractorId { get; set; }
}

public class UpdateWorkerDto
{
    public Guid Id { get; set; }
    public string WorkerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Trade { get; set; }
    public decimal DailyWageRate { get; set; }
    public DateTime JoinDate { get; set; }
    public Guid? ContractorId { get; set; }
}
