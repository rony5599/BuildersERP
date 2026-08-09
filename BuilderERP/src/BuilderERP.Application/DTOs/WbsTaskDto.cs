namespace BuilderERP.Application.DTOs;

public class WbsTaskDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PercentComplete { get; set; }
    public int Sequence { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentCode { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateWbsTaskDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PercentComplete { get; set; }
    public int Sequence { get; set; }
    public Guid? ParentId { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateWbsTaskDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PercentComplete { get; set; }
    public int Sequence { get; set; }
    public Guid? ParentId { get; set; }
    public Guid ProjectId { get; set; }
}
