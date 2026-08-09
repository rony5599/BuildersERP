namespace BuilderERP.Application.DTOs;

public class OperatorAssignmentDto
{
    public Guid Id { get; set; }
    public DateTime AssignmentStartDate { get; set; }
    public DateTime? AssignmentEndDate { get; set; }
    public bool IsActive { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateOperatorAssignmentDto
{
    public DateTime AssignmentStartDate { get; set; } = DateTime.UtcNow;
    public DateTime? AssignmentEndDate { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateOperatorAssignmentDto
{
    public Guid Id { get; set; }
    public DateTime AssignmentStartDate { get; set; }
    public DateTime? AssignmentEndDate { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
}
