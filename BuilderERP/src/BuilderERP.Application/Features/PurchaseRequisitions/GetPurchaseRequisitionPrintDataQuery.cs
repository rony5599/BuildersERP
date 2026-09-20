using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record GetPurchaseRequisitionPrintDataQuery(long Id) : IRequest<PurchaseRequisitionPrintDto?>;

public class GetPurchaseRequisitionPrintDataQueryHandler : IRequestHandler<GetPurchaseRequisitionPrintDataQuery, PurchaseRequisitionPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseRequisitionPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseRequisitionPrintDto?> Handle(GetPurchaseRequisitionPrintDataQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<PurchaseRequisition>().Query()
            .Include(r => r.Department)
            .Include(r => r.Project)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (requisition is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return new PurchaseRequisitionPrintDto
        {
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Status = requisition.Status,
            Description = requisition.Description,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            DepartmentName = requisition.Department?.Name ?? string.Empty,
            ProjectName = requisition.Project?.Name ?? string.Empty,
            Lines = requisition.Details.Select(d => new PurchaseRequisitionPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Rate = d.EstimatedUnitPrice,
                Amount = d.EstimatedAmount,
                Remarks = d.Remarks
            }).ToList()
        };
    }
}
