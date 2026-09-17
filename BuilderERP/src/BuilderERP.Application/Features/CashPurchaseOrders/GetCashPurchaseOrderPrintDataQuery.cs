using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record GetCashPurchaseOrderPrintDataQuery(long Id) : IRequest<CashPurchaseOrderPrintDto?>;

public class GetCashPurchaseOrderPrintDataQueryHandler : IRequestHandler<GetCashPurchaseOrderPrintDataQuery, CashPurchaseOrderPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCashPurchaseOrderPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CashPurchaseOrderPrintDto?> Handle(GetCashPurchaseOrderPrintDataQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<CashPurchaseOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.CashRequisition)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var supplier = order.Supplier;

        return new CashPurchaseOrderPrintDto
        {
            CPONumber = order.CPONumber,
            RequisitionNumber = order.CashRequisition?.RequisitionNumber ?? string.Empty,
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
            Lines = order.Details.Select(d => new CashPurchaseOrderPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.OrderedQuantity,
                Rate = d.UnitPrice,
                Amount = d.LineTotal
            }).ToList()
        };
    }
}
