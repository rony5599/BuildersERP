using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetVendorQuotationPrintDataQuery(long Id) : IRequest<VendorQuotationPrintDto?>;

public class GetVendorQuotationPrintDataQueryHandler : IRequestHandler<GetVendorQuotationPrintDataQuery, VendorQuotationPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVendorQuotationPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VendorQuotationPrintDto?> Handle(GetVendorQuotationPrintDataQuery request, CancellationToken cancellationToken)
    {
        var quotation = await _unitOfWork.Repository<VendorQuotation>().Query()
            .Include(v => v.Supplier)
            .Include(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(v => v.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (quotation is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var supplier = quotation.Supplier;
        var purchaseRequisition = quotation.Rfq?.PurchaseRequisition;

        return new VendorQuotationPrintDto
        {
            QuotationNumber = quotation.QuotationNumber,
            QuotationDate = quotation.QuotationDate,
            DeliveryDays = quotation.DeliveryDays,
            Status = quotation.Status,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            SupplierName = supplier?.Name ?? string.Empty,
            SupplierAddress = supplier?.Address,
            SupplierPhone = supplier?.Phone,
            SupplierEmail = supplier?.Email,
            RfqNumber = quotation.Rfq?.RfqNumber ?? string.Empty,
            RequisitionNumber = purchaseRequisition?.RequisitionNumber ?? string.Empty,
            ProjectName = purchaseRequisition?.Project?.Name ?? string.Empty,
            Lines = quotation.Details.Select(d => new VendorQuotationPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Rate = d.UnitPrice,
                Amount = d.NetAmount
            }).ToList()
        };
    }
}
