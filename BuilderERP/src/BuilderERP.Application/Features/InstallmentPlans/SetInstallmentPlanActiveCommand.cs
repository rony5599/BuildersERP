using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record SetInstallmentPlanActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetInstallmentPlanActiveCommandHandler : IRequestHandler<SetInstallmentPlanActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetInstallmentPlanActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetInstallmentPlanActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<InstallmentPlan>();
        var plan = await repository.GetByIdAsync(request.Id);
        if (plan is null)
        {
            return false;
        }

        plan.IsActive = request.IsActive;
        repository.Update(plan);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
