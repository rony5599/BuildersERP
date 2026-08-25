using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BudgetLines;

public record CreateBudgetLineCommand(CreateBudgetLineDto Dto) : IRequest<long>;

public class CreateBudgetLineCommandHandler : IRequestHandler<CreateBudgetLineCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBudgetLineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateBudgetLineCommand request, CancellationToken cancellationToken)
    {
        var budgetLine = _mapper.Map<BudgetLine>(request.Dto);
        await _unitOfWork.Repository<BudgetLine>().AddAsync(budgetLine);
        await _unitOfWork.SaveChangesAsync();
        return budgetLine.Id;
    }
}
