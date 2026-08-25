namespace BuilderERP.Application.DTOs;

public class OperatorAssignmentDto
{
    public long Id { get; set; }
    public DateTime AssignmentStartDate { get; set; }
    public DateTime? AssignmentEndDate { get; set; }
    public bool IsActive { get; set; }
    public long EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public long WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateOperatorAssignmentDto
{
    public DateTime AssignmentStartDate { get; set; } = DateTime.UtcNow;
    public DateTime? AssignmentEndDate { get; set; }
    public long EquipmentId { get; set; }
    public long WorkerId { get; set; }
    public long ProjectId { get; set; }
}

public class UpdateOperatorAssignmentDto
{
    public long Id { get; set; }
    public DateTime AssignmentStartDate { get; set; }
    public DateTime? AssignmentEndDate { get; set; }
    public long EquipmentId { get; set; }
    public long WorkerId { get; set; }
    public long ProjectId { get; set; }
}
