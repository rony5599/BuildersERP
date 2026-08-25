using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Receipt : BaseEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal AmountPaid { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public long InstallmentId { get; set; }
    public Installment Installment { get; set; } = null!;
}
