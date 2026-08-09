using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class Document : BaseEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; } = DocumentType.Other;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
}
