using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class CommissionDto
{
    public long Id { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public CommissionStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public long BrokerId { get; set; }
    public string BrokerName { get; set; } = string.Empty;
    public long BookingId { get; set; }
    public decimal BookingAmount { get; set; }
}

public class CreateCommissionDto
{
    public decimal CommissionRate { get; set; }
    public decimal? CommissionAmount { get; set; }
    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public string? Remarks { get; set; }
    public long BrokerId { get; set; }
    public long BookingId { get; set; }
}

public class UpdateCommissionDto
{
    public long Id { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal? CommissionAmount { get; set; }
    public CommissionStatus Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Remarks { get; set; }
    public long BrokerId { get; set; }
    public long BookingId { get; set; }
}
