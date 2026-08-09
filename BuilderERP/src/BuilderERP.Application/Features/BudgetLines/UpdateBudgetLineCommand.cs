using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BudgetLines;

public record UpdateBudgetLineCommand(UpdateBudgetLineDto Dto) : IRequest<bool>;

public class UpdateBudgetLineCommandHandler : IRequestHandler<UpdateBudgetLineCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBudgetLineCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBudgetLineCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BudgetLine>();
        var budgetLine = await repository.GetByIdAsync(request.Dto.Id);
        if (budgetLine is null)
        {
            return false;
        }

        budgetLine.Category = request.Dto.Category;
        budgetLine.PeriodStart = request.Dto.PeriodStart;
        budgetLine.BudgetedAmount = request.Dto.BudgetedAmount;
        budgetLine.ActualAmount = request.Dto.ActualAmount;
        budgetLine.Remarks = request.Dto.Remarks;
        budgetLine.ProjectId = request.Dto.ProjectId;

        repository.Update(budgetLine);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
