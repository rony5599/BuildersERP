using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.UtilityBills;

public record UpdateUtilityBillCommand(UpdateUtilityBillDto Dto) : IRequest<bool>;

public class UpdateUtilityBillCommandHandler : IRequestHandler<UpdateUtilityBillCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUtilityBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<UtilityBill>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.BillNumber = request.Dto.BillNumber;
        item.UtilityType = request.Dto.UtilityType;
        item.BillingMonth = request.Dto.BillingMonth;
        item.Amount = request.Dto.Amount;
        item.DueDate = request.Dto.DueDate;
        item.PaidDate = request.Dto.PaidDate;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
