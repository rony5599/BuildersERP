namespace BuilderERP.Application.DTOs;

public class InquiryDto
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; }
    public bool IsActive { get; set; }
    public long LeadId { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public long? PropertyUnitId { get; set; }
    public string? PropertyUnitNumber { get; set; }
}

public class CreateInquiryDto
{
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public long LeadId { get; set; }
    public long? PropertyUnitId { get; set; }
}

public class UpdateInquiryDto
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; }
    public long LeadId { get; set; }
    public long? PropertyUnitId { get; set; }
}
