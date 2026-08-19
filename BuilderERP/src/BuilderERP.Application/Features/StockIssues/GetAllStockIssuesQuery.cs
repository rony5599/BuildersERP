using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockIssues;

public record GetAllStockIssuesQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<StockIssueDto>>;

public class GetAllStockIssuesQueryHandler : IRequestHandler<GetAllStockIssuesQuery, PagedResult<StockIssueDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockIssuesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StockIssueDto>> Handle(GetAllStockIssuesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<StockIssue>().Query()
            .Include(i => i.Material)
            .Include(i => i.Warehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(i => i.Warehouse.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var issues = await query
            .OrderByDescending(i => i.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<StockIssueDto>>(issues);
        return new PagedResult<StockIssueDto>(items, totalCount, page, pageSize);
    }
}
