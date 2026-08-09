using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BudgetLines;

public record SetBudgetLineActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetBudgetLineActiveCommandHandler : IRequestHandler<SetBudgetLineActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBudgetLineActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBudgetLineActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BudgetLine>();
        var budgetLine = await repository.GetByIdAsync(request.Id);
        if (budgetLine is null)
        {
            return false;
        }

        budgetLine.IsActive = request.IsActive;
        repository.Update(budgetLine);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
