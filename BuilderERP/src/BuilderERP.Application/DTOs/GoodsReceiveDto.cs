namespace BuilderERP.Application.DTOs;

public class GoodsReceiveDto
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string PONumber { get; set; } = string.Empty;
}

public class CreateGoodsReceiveDto
{
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public Guid PurchaseOrderId { get; set; }
}

public class UpdateGoodsReceiveDto
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public string? Remarks { get; set; }
    public Guid PurchaseOrderId { get; set; }
}
