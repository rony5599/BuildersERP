using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RfqDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public List<RfqVendorDto> Vendors { get; set; } = new();
    public List<RfqDetailDto> Details { get; set; } = new();
}

public class RfqVendorDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime InvitedDate { get; set; }
    public RfqVendorStatus Status { get; set; }
}

public class RfqDetailDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public string? Specification { get; set; }
}

public class CreateRfqDetailDto
{
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public string? Specification { get; set; }
}

public class CreateRfqDto
{
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; } = RfqStatus.Sent;
    public Guid PurchaseRequisitionId { get; set; }
    public List<Guid> SupplierIds { get; set; } = new();
    public List<CreateRfqDetailDto> Details { get; set; } = new();
}

public class UpdateRfqDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public Guid PurchaseRequisitionId { get; set; }
    public List<Guid> SupplierIds { get; set; } = new();
    public List<CreateRfqDetailDto> Details { get; set; } = new();
}
