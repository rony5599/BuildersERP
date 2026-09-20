using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record GetBillablePurchaseOrdersQuery(long? IncludePurchaseOrderId = null) : IRequest<IReadOnlyList<BillablePurchaseOrderDto>>;

public class GetBillablePurchaseOrdersQueryHandler : IRequestHandler<GetBillablePurchaseOrdersQuery, IReadOnlyList<BillablePurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBillablePurchaseOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<BillablePurchaseOrderDto>> Handle(GetBillablePurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var statuses = PoBillLineBuilder.BillableStatuses;
        return await _unitOfWork.Repository<PurchaseOrder>().Query()
            .Where(o => (o.IsActive && statuses.Contains(o.Status)) || o.Id == request.IncludePurchaseOrderId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new BillablePurchaseOrderDto
            {
                Id = o.Id,
                PONumber = o.PONumber,
                SupplierName = o.VendorQuotation.Supplier.Name,
                ProjectName = o.VendorQuotation.Rfq.PurchaseRequisition.Project.Name
            })
            .ToListAsync(cancellationToken);
    }
}

public record GetPoBillableLinesQuery(long PurchaseOrderId, long? ExcludeBillId = null) : IRequest<IReadOnlyList<PoBillableLineDto>>;

public class GetPoBillableLinesQueryHandler : IRequestHandler<GetPoBillableLinesQuery, IReadOnlyList<PoBillableLineDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPoBillableLinesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PoBillableLineDto>> Handle(GetPoBillableLinesQuery request, CancellationToken cancellationToken)
    {
        var billed = await PoBillLineBuilder.BilledQuantitiesAsync(_unitOfWork, request.PurchaseOrderId, request.ExcludeBillId, cancellationToken);
        var lines = await _unitOfWork.Repository<PurchaseOrderDetail>().Query()
            .Where(d => d.PurchaseOrderId == request.PurchaseOrderId)
            .Select(d => new PoBillableLineDto
            {
                PurchaseOrderDetailId = d.Id,
                MaterialId = d.MaterialId,
                MaterialName = d.Material.Name,
                UnitOfMeasure = d.UnitOfMeasure,
                OrderedQuantity = d.OrderedQuantity,
                UnitPrice = d.UnitPrice,
                DiscountPercent = d.DiscountPercent,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent
            })
            .ToListAsync(cancellationToken);

        foreach (var line in lines)
        {
            billed.TryGetValue(line.PurchaseOrderDetailId, out var qty);
            line.AlreadyBilledQuantity = qty;
        }

        return lines;
    }
}
