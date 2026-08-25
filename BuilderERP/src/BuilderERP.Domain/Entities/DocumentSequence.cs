namespace BuilderERP.Domain.Entities;

public class DocumentSequence
{
    public long Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public long ProjectId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public int LastNumber { get; set; }
}
