using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityDeposits;

public record UpdateSecurityDepositCommand(UpdateSecurityDepositDto Dto) : IRequest<bool>;

public class UpdateSecurityDepositCommandHandler : IRequestHandler<UpdateSecurityDepositCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSecurityDepositCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSecurityDepositCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SecurityDeposit>();
        var deposit = await repository.GetByIdAsync(request.Dto.Id);
        if (deposit is null)
        {
            return false;
        }

        deposit.ContractorId = request.Dto.ContractorId;
        deposit.WorkOrderId = request.Dto.WorkOrderId;
        deposit.DepositAmount = request.Dto.DepositAmount;
        deposit.DepositDate = request.Dto.DepositDate;
        deposit.RefundDate = request.Dto.RefundDate;
        deposit.Status = request.Dto.Status;

        repository.Update(deposit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
