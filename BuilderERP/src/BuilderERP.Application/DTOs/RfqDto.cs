using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class RfqDto
{
    public long Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public bool IsActive { get; set; }
    public long PurchaseRequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<RfqVendorDto> Vendors { get; set; } = new();
    public List<RfqDetailDto> Details { get; set; } = new();
}

public class RfqVendorDto
{
    public long Id { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime InvitedDate { get; set; }
    public RfqVendorStatus Status { get; set; }
}

public class RfqDetailDto
{
    public long Id { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public string? Specification { get; set; }
}

public class CreateRfqDetailDto
{
    public long MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public string? Specification { get; set; }
}

public class CreateRfqDto
{
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; } = RfqStatus.Sent;
    public long PurchaseRequisitionId { get; set; }
    public List<long> SupplierIds { get; set; } = new();
    public List<CreateRfqDetailDto> Details { get; set; } = new();
}

public class UpdateRfqDto
{
    public long Id { get; set; }
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }
    public long PurchaseRequisitionId { get; set; }
    public List<long> SupplierIds { get; set; } = new();
    public List<CreateRfqDetailDto> Details { get; set; } = new();
}

public class RfqPrintDto
{
    public string RfqNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public RfqStatus Status { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;

    public string? PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }

    public List<string> VendorNames { get; set; } = new();
    public List<RfqPrintLineDto> Lines { get; set; } = new();
}

public class RfqPrintLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Specification { get; set; }
}
