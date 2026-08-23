using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class SaleAgreementDto
{
    public Guid Id { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public DateTime AgreementDate { get; set; }
    public decimal TotalSalePrice { get; set; }
    public AgreementStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid BookingId { get; set; }
    public string BookingUnitNumber { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateSaleAgreementDto
{
    public string AgreementNumber { get; set; } = string.Empty;
    public DateTime AgreementDate { get; set; } = DateTime.UtcNow;
    public decimal TotalSalePrice { get; set; }
    public AgreementStatus Status { get; set; } = AgreementStatus.Draft;
    public Guid BookingId { get; set; }
}

public class UpdateSaleAgreementDto
{
    public Guid Id { get; set; }
    public string AgreementNumber { get; set; } = string.Empty;
    public DateTime AgreementDate { get; set; }
    public decimal TotalSalePrice { get; set; }
    public AgreementStatus Status { get; set; }
    public Guid BookingId { get; set; }
}
