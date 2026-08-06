using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record UpdateInstallmentPlanCommand(UpdateInstallmentPlanDto Dto) : IRequest<bool>;

public class UpdateInstallmentPlanCommandHandler : IRequestHandler<UpdateInstallmentPlanCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInstallmentPlanCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateInstallmentPlanCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<InstallmentPlan>();
        var plan = await repository.GetByIdAsync(request.Dto.Id);
        if (plan is null)
        {
            return false;
        }

        plan.TotalAmount = request.Dto.TotalAmount;
        plan.NumberOfInstallments = request.Dto.NumberOfInstallments;
        plan.StartDate = request.Dto.StartDate;
        plan.InterestRatePercent = request.Dto.InterestRatePercent;
        plan.Status = request.Dto.Status;
        plan.SaleAgreementId = request.Dto.SaleAgreementId;

        repository.Update(plan);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
