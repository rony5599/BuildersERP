using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class ReceiptDto
{
    public long Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal AmountPaid { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public long InstallmentId { get; set; }
    public int InstallmentNumber { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateReceiptDto
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal AmountPaid { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public long InstallmentId { get; set; }
}

public class UpdateReceiptDto
{
    public long Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal AmountPaid { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public long InstallmentId { get; set; }
}
