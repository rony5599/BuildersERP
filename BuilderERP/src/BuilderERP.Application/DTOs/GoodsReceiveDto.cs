using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class GoodsReceiveDto
{
    public long Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public GrnStatus Status { get; set; }
    public bool IsActive { get; set; }
    public GrnSourceType SourceType { get; set; }
    public long? PurchaseOrderId { get; set; }
    public long? EngineerWorkOrderId { get; set; }
    public long? CashPurchaseOrderId { get; set; }
    public string SourceDocumentNumber { get; set; } = string.Empty;
    public string PONumber { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<GoodsReceiveDetailDto> Details { get; set; } = new();
}

public class GoodsReceiveDetailDto
{
    public long Id { get; set; }
    public long? PurchaseOrderDetailId { get; set; }
    public long? EngineerWorkOrderDetailId { get; set; }
    public long? CashPurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal ReceivedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public string? BatchNo { get; set; }
    public string? SerialNo { get; set; }
    public decimal VatPercent { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreateGoodsReceiveDetailDto
{
    public long? PurchaseOrderDetailId { get; set; }
    public long? EngineerWorkOrderDetailId { get; set; }
    public long? CashPurchaseOrderDetailId { get; set; }
    public long MaterialId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitPrice { get; set; }
    public string? BatchNo { get; set; }
    public string? SerialNo { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
}

public class CreateGoodsReceiveDto
{
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public GrnSourceType SourceType { get; set; } = GrnSourceType.PurchaseOrder;
    public long? PurchaseOrderId { get; set; }
    public long? EngineerWorkOrderId { get; set; }
    public long? CashPurchaseOrderId { get; set; }
    public long WarehouseId { get; set; }
    public List<CreateGoodsReceiveDetailDto> Details { get; set; } = new();
}

public class UpdateGoodsReceiveDto
{
    public long Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string? Remarks { get; set; }
    public GrnStatus Status { get; set; }
    public GrnSourceType SourceType { get; set; } = GrnSourceType.PurchaseOrder;
    public long? PurchaseOrderId { get; set; }
    public long? EngineerWorkOrderId { get; set; }
    public long? CashPurchaseOrderId { get; set; }
    public long WarehouseId { get; set; }
    public List<CreateGoodsReceiveDetailDto> Details { get; set; } = new();
}
