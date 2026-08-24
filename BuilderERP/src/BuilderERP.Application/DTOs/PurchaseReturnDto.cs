using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class PurchaseReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal ReturnAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid GoodsReceiveId { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<PurchaseReturnDetailDto> Details { get; set; } = new();
}

public class PurchaseReturnDetailDto
{
    public Guid Id { get; set; }
    public Guid GoodsReceiveDetailId { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal ReturnQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreatePurchaseReturnDetailDto
{
    public Guid GoodsReceiveDetailId { get; set; }
    public Guid MaterialId { get; set; }
    public decimal ReturnQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}

public class CreatePurchaseReturnDto
{
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; } = PurchaseReturnStatus.Pending;
    public Guid GoodsReceiveId { get; set; }
    public List<CreatePurchaseReturnDetailDto> Details { get; set; } = new();
}

public class UpdatePurchaseReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; }
    public Guid GoodsReceiveId { get; set; }
    public List<CreatePurchaseReturnDetailDto> Details { get; set; } = new();
}
