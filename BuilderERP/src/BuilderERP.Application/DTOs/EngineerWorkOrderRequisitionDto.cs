using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class EngineerWorkOrderRequisitionDto
{
    public long Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public RequisitionStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<EngineerWorkOrderRequisitionDetailDto> Details { get; set; } = new();
}

public class EngineerWorkOrderRequisitionDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedAmount { get; set; }
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderRequisitionDetailDto
{
    public long MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal EstimatedUnitPrice { get; set; }
    public string? Remarks { get; set; }
}

public class CreateEngineerWorkOrderRequisitionDto
{
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
    public long ProjectId { get; set; }
    public List<CreateEngineerWorkOrderRequisitionDetailDto> Details { get; set; } = new();
}

public class UpdateEngineerWorkOrderRequisitionDto
{
    public long Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public string? Description { get; set; }
    public RequisitionStatus Status { get; set; }
    public long ProjectId { get; set; }
    public List<CreateEngineerWorkOrderRequisitionDetailDto> Details { get; set; } = new();
}

public class EngineerWorkOrderRequisitionPrintDto
{
    public string RequisitionNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime RequiredByDate { get; set; }
    public RequisitionStatus Status { get; set; }
    public string? Description { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<EngineerWorkOrderRequisitionPrintLineDto> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.Amount);
}

public class EngineerWorkOrderRequisitionPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }
}
