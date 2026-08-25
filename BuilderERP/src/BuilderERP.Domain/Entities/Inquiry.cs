namespace BuilderERP.Domain.Entities;

public class Inquiry : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public long LeadId { get; set; }
    public Lead Lead { get; set; } = null!;

    public long? PropertyUnitId { get; set; }
    public PropertyUnit? PropertyUnit { get; set; }
}
