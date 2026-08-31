using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record UpdatePurchaseOrderCommand(UpdatePurchaseOrderDto Dto) : IRequest<UpdatePurchaseOrderResult>;

public enum UpdatePurchaseOrderResult
{
    Success,
    NotFound,
    Locked
}

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, UpdatePurchaseOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePurchaseOrderResult> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseOrder>();
        var order = await repository.Query()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == request.Dto.Id, cancellationToken);

        if (order is null)
        {
            return UpdatePurchaseOrderResult.NotFound;
        }

        if (order.Status != PurchaseOrderStatus.Draft)
        {
            return UpdatePurchaseOrderResult.Locked;
        }

        order.PONumber = request.Dto.PONumber;
        order.OrderDate = request.Dto.OrderDate;
        order.DeliveryDate = request.Dto.DeliveryDate;
        order.Status = request.Dto.Status;
        order.TermsOfPayment = request.Dto.TermsOfPayment;
        order.DispatchedThrough = request.Dto.DispatchedThrough;
        order.Destination = request.Dto.Destination;
        order.Remarks = request.Dto.Remarks;
        order.VendorQuotationId = request.Dto.VendorQuotationId;

        var detailRepository = _unitOfWork.Repository<PurchaseOrderDetail>();
        foreach (var detail in order.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.OrderedQuantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            await detailRepository.AddAsync(new PurchaseOrderDetail
            {
                PurchaseOrderId = order.Id,
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
        return UpdatePurchaseOrderResult.Success;
    }
}
