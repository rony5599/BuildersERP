using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record UpdateCashPurchaseOrderCommand(UpdateCashPurchaseOrderDto Dto) : IRequest<UpdateCashPurchaseOrderResult>;

public enum UpdateCashPurchaseOrderResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateCashPurchaseOrderCommandHandler : IRequestHandler<UpdateCashPurchaseOrderCommand, UpdateCashPurchaseOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCashPurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCashPurchaseOrderResult> Handle(UpdateCashPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashPurchaseOrder>();
        var order = await repository.Query()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == request.Dto.Id, cancellationToken);

        if (order is null)
        {
            return UpdateCashPurchaseOrderResult.NotFound;
        }

        if (order.Status != PurchaseOrderStatus.Draft)
        {
            return UpdateCashPurchaseOrderResult.Locked;
        }

        order.CPONumber = request.Dto.CPONumber;
        order.OrderDate = request.Dto.OrderDate;
        order.DeliveryDate = request.Dto.DeliveryDate;
        order.Status = request.Dto.Status;
        order.TermsOfPayment = request.Dto.TermsOfPayment;
        order.DispatchedThrough = request.Dto.DispatchedThrough;
        order.Destination = request.Dto.Destination;
        order.Remarks = request.Dto.Remarks;
        order.SupplierId = request.Dto.SupplierId;
        order.CashRequisitionId = request.Dto.CashRequisitionId;

        var detailRepository = _unitOfWork.Repository<CashPurchaseOrderDetail>();
        foreach (var detail in order.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.OrderedQuantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            await detailRepository.AddAsync(new CashPurchaseOrderDetail
            {
                CashPurchaseOrderId = order.Id,
                MaterialId = detail.MaterialId,
                OrderedQuantity = detail.OrderedQuantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                UnitPrice = detail.UnitPrice,
                DiscountPercent = detail.DiscountPercent,
                DiscountAmount = amounts.DiscountAmount,
                VatPercent = detail.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = detail.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                LineTotal = amounts.NetAmount
            });
            totalAmount += amounts.NetAmount;
        }

        order.TotalAmount = totalAmount;

        repository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return UpdateCashPurchaseOrderResult.Success;
    }
}
