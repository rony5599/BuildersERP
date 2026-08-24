using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class GoodsReceiveDto
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public GrnStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<GoodsReceiveDetailDto> Details { get; set; } = new();
}

public class GoodsReceiveDetailDto
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderDetailId { get; set; }
    public Guid MaterialId { get; set; }
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
    public Guid PurchaseOrderDetailId { get; set; }
    public Guid MaterialId { get; set; }
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
    public Guid PurchaseOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public List<CreateGoodsReceiveDetailDto> Details { get; set; } = new();
}

public class UpdateGoodsReceiveDto
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string? Remarks { get; set; }
    public GrnStatus Status { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public List<CreateGoodsReceiveDetailDto> Details { get; set; } = new();
}
