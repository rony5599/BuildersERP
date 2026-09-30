using BuilderERP.Application.Common.Caching;
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

public record CreateSupplierPaymentCommand(CreateSupplierPaymentDto Dto) : IRequest<CreateSupplierPaymentResult>, IInvalidatesFeatures
{
    // EWO bill grid and statement show the paid amount of each bill.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["EwoBills"];
}

public class CreateSupplierPaymentCommandHandler : IRequestHandler<CreateSupplierPaymentCommand, CreateSupplierPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateSupplierPaymentCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    private record PayableBill(PoBillStatus Status, bool IsActive, decimal Amount, long SupplierId, long ProjectId);

    public async Task<CreateSupplierPaymentResult> Handle(CreateSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var bill = dto.EwoBillId is > 0
            ? await _unitOfWork.Repository<EwoBill>().Query()
                .Where(b => b.Id == dto.EwoBillId)
                .Select(b => new PayableBill(b.Status, b.IsActive, b.NetPayable, b.SupplierId, b.EngineerWorkOrder.EngineerWorkOrderRequisition.ProjectId))
                .FirstOrDefaultAsync(cancellationToken)
            : await _unitOfWork.Repository<PoBill>().Query()
                .Where(b => b.Id == dto.PoBillId)
                .Select(b => new PayableBill(b.Status, b.IsActive, b.TotalAmount,
                    b.PurchaseOrder.VendorQuotation.SupplierId, b.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId))
                .FirstOrDefaultAsync(cancellationToken);

        if (bill is null || !bill.IsActive || bill.Status != PoBillStatus.Approved)
        {
            return CreateSupplierPaymentResult.BillNotPayable;
        }

        var billPayments = _unitOfWork.Repository<SupplierPayment>().Query().Where(p => p.IsActive);
        billPayments = dto.EwoBillId is > 0
            ? billPayments.Where(p => p.EwoBillId == dto.EwoBillId)
            : billPayments.Where(p => p.PoBillId == dto.PoBillId);
        var paid = await billPayments.SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0;

        if (dto.Amount > bill.Amount - paid)
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
            PoBillId = dto.EwoBillId > 0 ? null : dto.PoBillId,
            EwoBillId = dto.EwoBillId > 0 ? dto.EwoBillId : null,
            SupplierId = bill.SupplierId
        };

        await _unitOfWork.Repository<SupplierPayment>().AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        return CreateSupplierPaymentResult.Success;
    }
}

public record SetSupplierPaymentActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["EwoBills"];
}

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
