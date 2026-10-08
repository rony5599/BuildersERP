using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetAllBoqsQuery(long? ProjectId = null, string? Search = null, int Page = 1, int PageSize = 25)
    : IRequest<PagedResult<BoqSummaryDto>>;

public class GetAllBoqsQueryHandler : IRequestHandler<GetAllBoqsQuery, PagedResult<BoqSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBoqsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<BoqSummaryDto>> Handle(GetAllBoqsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BoqHeader>().Query().AsNoTracking();
        if (request.ProjectId.HasValue)
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.BoqName.Contains(search));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Max(1, request.PageSize);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BoqSummaryDto
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                BoqName = x.BoqName,
                VersionNumber = x.VersionNumber,
                CreatedAt = x.CreatedAt,
                ItemCount = x.Items.Count,
                Subtotal = x.Items.Sum(i => i.Quantity * i.Rate),
                ContingencyPercent = x.ContingencyPercent
                ,IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<BoqSummaryDto>(items, totalCount, page, pageSize);
    }
}
