using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RunningBills;

public record UpdateRunningBillCommand(UpdateRunningBillDto Dto) : IRequest<bool>;

public class UpdateRunningBillCommandHandler : IRequestHandler<UpdateRunningBillCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRunningBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRunningBillCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RunningBill>();
        var bill = await repository.GetByIdAsync(request.Dto.Id);
        if (bill is null)
        {
            return false;
        }

        bill.BillNumber = request.Dto.BillNumber;
        bill.BillDate = request.Dto.BillDate;
        bill.WorkDoneAmount = request.Dto.WorkDoneAmount;
        bill.PreviousBillAmount = request.Dto.PreviousBillAmount;
        bill.DeductionAmount = request.Dto.DeductionAmount;
        bill.Status = request.Dto.Status;
        bill.WorkOrderId = request.Dto.WorkOrderId;
        bill.NetPayableAmount = bill.WorkDoneAmount - bill.PreviousBillAmount - bill.DeductionAmount;

        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
