using BuilderERP.Application.Common.Caching;
using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record CreateCashPurchaseOrderCommand(CreateCashPurchaseOrderDto Dto) : IRequest<long>, IInvalidatesFeatures
{
    // Also read by other features' cached queries: the billable-CPO dropdown and billed quantities are cached under CashPoBills; the requester ledger is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["CashPoBills", "RequesterLedger"];
}

public class CreateCashPurchaseOrderCommandHandler : IRequestHandler<CreateCashPurchaseOrderCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateCashPurchaseOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreateCashPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<CashRequisition>().Query()
            .Where(r => r.Id == request.Dto.CashRequisitionId && r.Status == RequisitionStatus.Approved)
            .Select(r => r.ProjectId)
            .SingleAsync(cancellationToken);

        var order = _mapper.Map<CashPurchaseOrder>(request.Dto);
        order.CPONumber = await _numberGenerator.GenerateAsync(projectId, "CPO", cancellationToken);

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.OrderedQuantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            order.Details.Add(new CashPurchaseOrderDetail
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

        await _unitOfWork.Repository<CashPurchaseOrder>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return order.Id;
    }
}
