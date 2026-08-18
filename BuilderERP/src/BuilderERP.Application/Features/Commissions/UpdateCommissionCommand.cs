using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Commissions;

public record UpdateCommissionCommand(UpdateCommissionDto Dto) : IRequest<bool>;

public class UpdateCommissionCommandHandler : IRequestHandler<UpdateCommissionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommissionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Commission>();
        var commission = await repository.GetByIdAsync(request.Dto.Id);
        if (commission is null)
        {
            return false;
        }

        commission.CommissionRate = request.Dto.CommissionRate;
        commission.CommissionAmount = await CommissionCalculator.ResolveAmountAsync(_unitOfWork, request.Dto.BookingId, request.Dto.CommissionRate, request.Dto.CommissionAmount);
        commission.Status = request.Dto.Status;
        commission.PaymentDate = request.Dto.PaymentDate;
        commission.Remarks = request.Dto.Remarks;
        commission.BrokerId = request.Dto.BrokerId;
        commission.BookingId = request.Dto.BookingId;

        repository.Update(commission);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
