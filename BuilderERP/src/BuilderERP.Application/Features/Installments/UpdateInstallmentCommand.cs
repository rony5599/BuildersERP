using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record UpdateInstallmentCommand(UpdateInstallmentDto Dto) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["CollectionForecast"];
}

public class UpdateInstallmentCommandHandler : IRequestHandler<UpdateInstallmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInstallmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateInstallmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Installment>();
        var installment = await repository.GetByIdAsync(request.Dto.Id);
        if (installment is null)
        {
            return false;
        }

        installment.InstallmentNumber = request.Dto.InstallmentNumber;
        installment.DueDate = request.Dto.DueDate;
        installment.DueAmount = request.Dto.DueAmount;
        installment.PenaltyAmount = request.Dto.PenaltyAmount;
        installment.PaidAmount = request.Dto.PaidAmount;
        installment.Status = request.Dto.Status;
        installment.InstallmentPlanId = request.Dto.InstallmentPlanId;

        repository.Update(installment);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
