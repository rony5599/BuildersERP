using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class BoqItemDto
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public long BoqId { get; set; }
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public long WorkGroupId { get; set; }
    public string WorkGroupCode { get; set; } = string.Empty;
    public string WorkGroupName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateBoqItemDto
{
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public long BoqId { get; set; }
    public long WorkGroupId { get; set; }
}

public class UpdateBoqItemDto
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public long BoqId { get; set; }
    public long WorkGroupId { get; set; }
}

public class CreateBoqDto
{
    public long ProjectId { get; set; }
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; } = 1;
    public decimal ContingencyPercent { get; set; } = 2m;
    public List<CreateBoqItemDto> Items { get; set; } = new();
}

public class UpdateBoqDto : CreateBoqDto
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public BoqStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
}

public class BoqSummaryDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ContingencyPercent { get; set; }
    public decimal ContingencyAmount => Subtotal * ContingencyPercent / 100m;
    public decimal EstimatedTotal => Subtotal + ContingencyAmount;
    public bool IsActive { get; set; }
    public BoqStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class WorkGroupOptionDto
{
    public long Id { get; set; }
    public long? ParentGroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int Depth { get; set; }
    public string DisplayName => $"{new string('—', Depth)}{(Depth > 0 ? " " : "")}{GroupCode} — {GroupName}";
}

public class BoqPrintDto
{
    public long Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }
    public string BoqName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public BoqStatus Status { get; set; }
    public decimal ContingencyPercent { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
    public List<BoqPrintLineDto> Lines { get; set; } = new();
    public decimal Subtotal => Lines.Sum(x => x.Amount);
    public decimal ContingencyAmount => Subtotal * ContingencyPercent / 100m;
    public decimal EstimatedTotal => Subtotal + ContingencyAmount;
}

public class BoqPrintLineDto
{
    public string WorkGroupCode { get; set; } = string.Empty;
    public string WorkGroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount => Quantity * Rate;
}
