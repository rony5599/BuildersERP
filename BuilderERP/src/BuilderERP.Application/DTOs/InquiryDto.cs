namespace BuilderERP.Application.DTOs;

public class InquiryDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; }
    public bool IsActive { get; set; }
    public Guid LeadId { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public Guid? PropertyUnitId { get; set; }
    public string? PropertyUnitNumber { get; set; }
}

public class CreateInquiryDto
{
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public Guid LeadId { get; set; }
    public Guid? PropertyUnitId { get; set; }
}

public class UpdateInquiryDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime InquiryDate { get; set; }
    public Guid LeadId { get; set; }
    public Guid? PropertyUnitId { get; set; }
}
