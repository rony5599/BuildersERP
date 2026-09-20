using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record GetPoBillPrintDataQuery(long Id) : IRequest<PoBillPrintDto?>;

public class GetPoBillPrintDataQueryHandler : IRequestHandler<GetPoBillPrintDataQuery, PoBillPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPoBillPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PoBillPrintDto?> Handle(GetPoBillPrintDataQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<PoBill>().Query()
            .Include(b => b.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Supplier)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (bill is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var supplier = bill.PurchaseOrder.VendorQuotation?.Supplier;

        return new PoBillPrintDto
        {
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            DueDate = bill.DueDate,
            SupplierInvoiceNumber = bill.SupplierInvoiceNumber,
            PONumber = bill.PurchaseOrder.PONumber,
            PODate = bill.PurchaseOrder.OrderDate,
            Status = bill.Status,
            Remarks = bill.Remarks,
            PreparedBy = bill.CreatedBy,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            SupplierName = supplier?.Name ?? string.Empty,
            SupplierAddress = supplier?.Address,
            SupplierPhone = supplier?.Phone,
            SupplierEmail = supplier?.Email,
            Lines = bill.Details.Select(d => new PoBillPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.BilledQuantity,
                Rate = d.UnitPrice,
                Discount = d.DiscountAmount,
                Vat = d.VatAmount,
                Tax = d.TaxAmount,
                Amount = d.LineTotal
            }).ToList()
        };
    }
}
