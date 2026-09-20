using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record GetRfqPrintDataQuery(long Id) : IRequest<RfqPrintDto?>;

public class GetRfqPrintDataQueryHandler : IRequestHandler<GetRfqPrintDataQuery, RfqPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRfqPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RfqPrintDto?> Handle(GetRfqPrintDataQuery request, CancellationToken cancellationToken)
    {
        var rfq = await _unitOfWork.Repository<Rfq>().Query()
            .Include(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(r => r.RfqVendors).ThenInclude(v => v.Supplier)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (rfq is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return new RfqPrintDto
        {
            RfqNumber = rfq.RfqNumber,
            IssueDate = rfq.IssueDate,
            ClosingDate = rfq.ClosingDate,
            Status = rfq.Status,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            RequisitionNumber = rfq.PurchaseRequisition?.RequisitionNumber ?? string.Empty,
            ProjectName = rfq.PurchaseRequisition?.Project?.Name ?? string.Empty,
            VendorNames = rfq.RfqVendors.Select(v => v.Supplier?.Name ?? string.Empty).Where(n => !string.IsNullOrWhiteSpace(n)).ToList(),
            Lines = rfq.Details.Select(d => new RfqPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Specification = d.Specification
            }).ToList()
        };
    }
}
