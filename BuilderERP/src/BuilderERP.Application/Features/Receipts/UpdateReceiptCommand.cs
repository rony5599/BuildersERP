using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Receipts;

public record UpdateReceiptCommand(UpdateReceiptDto Dto) : IRequest<bool>;

public class UpdateReceiptCommandHandler : IRequestHandler<UpdateReceiptCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReceiptCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateReceiptCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Receipt>();
        var receipt = await repository.GetByIdAsync(request.Dto.Id);
        if (receipt is null)
        {
            return false;
        }

        var previousAmount = receipt.AmountPaid;
        var previousInstallmentId = receipt.InstallmentId;

        receipt.ReceiptNumber = request.Dto.ReceiptNumber;
        receipt.PaymentDate = request.Dto.PaymentDate;
        receipt.AmountPaid = request.Dto.AmountPaid;
        receipt.PaymentMethod = request.Dto.PaymentMethod;
        receipt.Notes = request.Dto.Notes;
        receipt.InstallmentId = request.Dto.InstallmentId;

        repository.Update(receipt);

        if (previousInstallmentId == request.Dto.InstallmentId)
        {
            var delta = request.Dto.AmountPaid - previousAmount;
            await ReceiptInstallmentSync.ApplyAsync(_unitOfWork, request.Dto.InstallmentId, delta);
        }
        else
        {
            await ReceiptInstallmentSync.ApplyAsync(_unitOfWork, previousInstallmentId, -previousAmount);
            await ReceiptInstallmentSync.ApplyAsync(_unitOfWork, request.Dto.InstallmentId, request.Dto.AmountPaid);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
