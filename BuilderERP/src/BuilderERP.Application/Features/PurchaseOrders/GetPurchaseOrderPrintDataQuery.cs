using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record GetPurchaseOrderPrintDataQuery(long Id) : IRequest<PurchaseOrderPrintDto?>;

public class GetPurchaseOrderPrintDataQueryHandler : IRequestHandler<GetPurchaseOrderPrintDataQuery, PurchaseOrderPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseOrderPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderPrintDto?> Handle(GetPurchaseOrderPrintDataQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<PurchaseOrder>().Query()
            .Include(o => o.VendorQuotation).ThenInclude(v => v.Supplier)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var supplier = order.VendorQuotation?.Supplier;

        return new PurchaseOrderPrintDto
        {
            PONumber = order.PONumber,
            OrderDate = order.OrderDate,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            SupplierName = supplier?.Name ?? string.Empty,
            SupplierAddress = supplier?.Address,
            SupplierPhone = supplier?.Phone,
            SupplierEmail = supplier?.Email,
            TermsOfPayment = order.TermsOfPayment,
            DispatchedThrough = order.DispatchedThrough,
            Destination = order.Destination,
            Remarks = order.Remarks,
            Lines = order.Details.Select(d => new PurchaseOrderPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.OrderedQuantity,
                Rate = d.UnitPrice,
                Amount = d.LineTotal
            }).ToList()
        };
    }
}
