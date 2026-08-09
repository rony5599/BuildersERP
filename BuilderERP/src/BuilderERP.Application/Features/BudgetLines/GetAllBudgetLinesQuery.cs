using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BudgetLines;

public record GetAllBudgetLinesQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<BudgetLineDto>>;

public class GetAllBudgetLinesQueryHandler : IRequestHandler<GetAllBudgetLinesQuery, IReadOnlyList<BudgetLineDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBudgetLinesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BudgetLineDto>> Handle(GetAllBudgetLinesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BudgetLine>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var budgetLines = await query
            .OrderByDescending(x => x.PeriodStart)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<BudgetLineDto>>(budgetLines);
    }
}
