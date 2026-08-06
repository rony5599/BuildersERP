using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class SaleAgreement : BaseEntity
{
    public string AgreementNumber { get; set; } = string.Empty;
    public DateTime AgreementDate { get; set; } = DateTime.UtcNow;
    public decimal TotalSalePrice { get; set; }
    public AgreementStatus Status { get; set; } = AgreementStatus.Draft;
    public bool IsActive { get; set; } = true;

    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}
