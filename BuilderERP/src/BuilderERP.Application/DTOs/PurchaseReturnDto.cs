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
}

public class CreatePurchaseReturnDto
{
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public decimal ReturnAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; } = PurchaseReturnStatus.Pending;
    public Guid GoodsReceiveId { get; set; }
}

public class UpdatePurchaseReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public decimal ReturnAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; }
    public Guid GoodsReceiveId { get; set; }
}
