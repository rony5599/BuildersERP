using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SupplierPayments;

public enum CreateSupplierPaymentResult
{
    Success,
    BillNotPayable,
    ExceedsOutstanding
}

public record CreateSupplierPaymentCommand(CreateSupplierPaymentDto Dto) : IRequest<CreateSupplierPaymentResult>;

public class CreateSupplierPaymentCommandHandler : IRequestHandler<CreateSupplierPaymentCommand, CreateSupplierPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateSupplierPaymentCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<CreateSupplierPaymentResult> Handle(CreateSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var bill = await _unitOfWork.Repository<PoBill>().Query()
            .Where(b => b.Id == dto.PoBillId)
            .Select(b => new
            {
                b.Status,
                b.IsActive,
                b.TotalAmount,
                SupplierId = b.PurchaseOrder.VendorQuotation.SupplierId,
                ProjectId = b.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bill is null || !bill.IsActive || bill.Status != PoBillStatus.Approved)
        {
            return CreateSupplierPaymentResult.BillNotPayable;
        }

        var paid = await _unitOfWork.Repository<SupplierPayment>().Query()
            .Where(p => p.PoBillId == dto.PoBillId && p.IsActive)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0;

        if (dto.Amount > bill.TotalAmount - paid)
        {
            return CreateSupplierPaymentResult.ExceedsOutstanding;
        }

        var payment = new SupplierPayment
        {
            PaymentNumber = await _numberGenerator.GenerateAsync(bill.ProjectId, "SPAY", cancellationToken),
            PaymentDate = dto.PaymentDate,
            Amount = dto.Amount,
            Method = dto.Method,
            ReferenceNumber = dto.ReferenceNumber,
            Remarks = dto.Remarks,
            PoBillId = dto.PoBillId,
            SupplierId = bill.SupplierId
        };

        await _unitOfWork.Repository<SupplierPayment>().AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        return CreateSupplierPaymentResult.Success;
    }
}

public record SetSupplierPaymentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSupplierPaymentActiveCommandHandler : IRequestHandler<SetSupplierPaymentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSupplierPaymentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSupplierPaymentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SupplierPayment>();
        var payment = await repository.GetByIdAsync(request.Id);
        if (payment is null)
        {
            return false;
        }

        payment.IsActive = request.IsActive;
        repository.Update(payment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
