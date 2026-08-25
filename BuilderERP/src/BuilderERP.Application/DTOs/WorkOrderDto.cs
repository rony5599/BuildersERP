using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class WorkOrderDto
{
    public long Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal Amount { get; set; }
    public WorkOrderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateWorkOrderDto
{
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; }
    public decimal Amount { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
    public long ContractorId { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateWorkOrderDto
{
    public long Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal Amount { get; set; }
    public WorkOrderStatus Status { get; set; }
    public long ContractorId { get; set; }
    public long ProjectId { get; set; }
}
