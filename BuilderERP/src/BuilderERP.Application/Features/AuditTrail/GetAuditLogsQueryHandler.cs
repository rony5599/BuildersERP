using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.AuditTrail;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, AuditLogPagedResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuditLogPagedResult> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<AuditLog>().Query();
        var filter = request.Filter;

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            query = query.Where(a => a.EntityName == filter.EntityName);
        }

        if (filter.EntityId.HasValue)
        {
            query = query.Where(a => a.EntityId == filter.EntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.ChangedBy))
        {
            query = query.Where(a => a.ChangedBy != null && a.ChangedBy.Contains(filter.ChangedBy));
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(a => a.ChangedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            var inclusiveEnd = filter.DateTo.Value.Date.AddDays(1);
            query = query.Where(a => a.ChangedAt < inclusiveEnd);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 25 : request.PageSize;

        query = query.OrderByDescending(a => a.ChangedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                PropertyName = a.PropertyName,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                ChangedBy = a.ChangedBy,
                ChangedAt = a.ChangedAt
            })
            .ToListAsync(cancellationToken);

        return new AuditLogPagedResult
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

public class GetAuditEntityNamesQueryHandler : IRequestHandler<GetAuditEntityNamesQuery, IReadOnlyList<string>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditEntityNamesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<string>> Handle(GetAuditEntityNamesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<AuditLog>().Query()
            .Select(a => a.EntityName)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}
