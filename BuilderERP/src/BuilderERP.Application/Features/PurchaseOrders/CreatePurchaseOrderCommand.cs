using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto Dto) : IRequest<long>;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<VendorQuotation>().Query()
            .Where(q => q.Id == request.Dto.VendorQuotationId)
            .Select(q => q.Rfq.PurchaseRequisition.ProjectId)
            .SingleAsync(cancellationToken);

        var order = _mapper.Map<PurchaseOrder>(request.Dto);
        order.PONumber = await _numberGenerator.GenerateAsync(projectId, "PO", cancellationToken);

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.OrderedQuantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            order.Details.Add(new PurchaseOrderDetail
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

        await _unitOfWork.Repository<PurchaseOrder>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return order.Id;
    }
}
