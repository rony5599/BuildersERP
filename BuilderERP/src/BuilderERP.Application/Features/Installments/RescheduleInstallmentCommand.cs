using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record RescheduleInstallmentCommand(Guid Id, DateTime NewDueDate, string Reason) : IRequest<bool>;

public class RescheduleInstallmentCommandHandler : IRequestHandler<RescheduleInstallmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleInstallmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RescheduleInstallmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Installment>();
        var installment = await repository.GetByIdAsync(request.Id);
        if (installment is null)
        {
            return false;
        }

        installment.OriginalDueDate ??= installment.DueDate;
        installment.DueDate = request.NewDueDate;
        installment.IsRescheduled = true;
        installment.RescheduleReason = request.Reason;
        if (installment.Status == InstallmentStatus.Overdue)
        {
            installment.Status = installment.PaidAmount > 0 ? InstallmentStatus.PartiallyPaid : InstallmentStatus.Pending;
        }

        repository.Update(installment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
