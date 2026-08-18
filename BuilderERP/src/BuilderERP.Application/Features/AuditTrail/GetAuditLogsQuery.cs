using MediatR;

namespace BuilderERP.Application.Features.AuditTrail;

public class AuditLogFilter
{
    public string? EntityName { get; set; }
    public Guid? EntityId { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public record GetAuditLogsQuery(AuditLogFilter Filter, int Page = 1, int PageSize = 25) : IRequest<AuditLogPagedResult>;

public record GetAuditEntityNamesQuery : IRequest<IReadOnlyList<string>>;
