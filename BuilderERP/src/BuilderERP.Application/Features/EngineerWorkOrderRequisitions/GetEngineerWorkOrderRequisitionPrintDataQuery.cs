using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record GetEngineerWorkOrderRequisitionPrintDataQuery(long Id) : IRequest<EngineerWorkOrderRequisitionPrintDto?>;

public class GetEngineerWorkOrderRequisitionPrintDataQueryHandler : IRequestHandler<GetEngineerWorkOrderRequisitionPrintDataQuery, EngineerWorkOrderRequisitionPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEngineerWorkOrderRequisitionPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EngineerWorkOrderRequisitionPrintDto?> Handle(GetEngineerWorkOrderRequisitionPrintDataQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<EngineerWorkOrderRequisition>().Query()
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

        return new EngineerWorkOrderRequisitionPrintDto
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
            ProjectName = requisition.Project?.Name ?? string.Empty,
            Lines = requisition.Details.Select(d => new EngineerWorkOrderRequisitionPrintLineDto
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
