using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BudgetLines;

public record GetAllBudgetLinesQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<BudgetLineDto>>;

public class GetAllBudgetLinesQueryHandler : IRequestHandler<GetAllBudgetLinesQuery, PagedResult<BudgetLineDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBudgetLinesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BudgetLineDto>> Handle(GetAllBudgetLinesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BudgetLine>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var budgetLines = await query
            .OrderByDescending(x => x.PeriodStart)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<BudgetLineDto>>(budgetLines);
        return new PagedResult<BudgetLineDto>(items, totalCount, page, pageSize);
    }
}
