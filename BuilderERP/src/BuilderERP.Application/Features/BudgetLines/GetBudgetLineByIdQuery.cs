using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BudgetLines;

public record GetBudgetLineByIdQuery(Guid Id) : IRequest<BudgetLineDto?>;

public class GetBudgetLineByIdQueryHandler : IRequestHandler<GetBudgetLineByIdQuery, BudgetLineDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBudgetLineByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BudgetLineDto?> Handle(GetBudgetLineByIdQuery request, CancellationToken cancellationToken)
    {
        var budgetLine = await _unitOfWork.Repository<BudgetLine>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return budgetLine is null ? null : _mapper.Map<BudgetLineDto>(budgetLine);
    }
}
