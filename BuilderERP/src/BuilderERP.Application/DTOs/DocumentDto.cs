using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class DocumentDto
{
    public long Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public long? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
}

public class CreateDocumentDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; } = DocumentType.Other;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public long? CustomerId { get; set; }
    public long? ProjectId { get; set; }
}

public class UpdateDocumentDto
{
    public long Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public long? CustomerId { get; set; }
    public long? ProjectId { get; set; }
}
