namespace BuilderERP.Domain.Entities;

public enum AuditAction
{
    Created,
    Modified,
    Deleted
}

public class AuditLog
{
    public long Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string EntityName { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public AuditAction Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
