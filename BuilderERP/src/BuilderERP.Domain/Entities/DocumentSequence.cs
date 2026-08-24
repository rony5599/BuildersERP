namespace BuilderERP.Domain.Entities;

public class DocumentSequence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public int LastNumber { get; set; }
}
