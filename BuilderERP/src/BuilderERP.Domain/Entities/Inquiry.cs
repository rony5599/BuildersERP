namespace BuilderERP.Domain.Entities;

public class Inquiry : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Guid LeadId { get; set; }
    public Lead Lead { get; set; } = null!;

    public Guid? PropertyUnitId { get; set; }
    public PropertyUnit? PropertyUnit { get; set; }
}
