using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashRequisitions;

public record GetCashRequisitionPrintDataQuery(long Id) : IRequest<CashRequisitionPrintDto?>;

public class GetCashRequisitionPrintDataQueryHandler : IRequestHandler<GetCashRequisitionPrintDataQuery, CashRequisitionPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCashRequisitionPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CashRequisitionPrintDto?> Handle(GetCashRequisitionPrintDataQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<CashRequisition>().Query()
            .Include(r => r.RequesterEmployee)
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

        return new CashRequisitionPrintDto
        {
            RequisitionNumber = requisition.RequisitionNumber,
            RequestDate = requisition.RequestDate,
            RequiredByDate = requisition.RequiredByDate,
            Status = requisition.Status,
            Description = requisition.Description,
            RequesterEmployeeName = requisition.RequesterEmployee?.EmployeeName ?? string.Empty,
            PaymentMethod = requisition.PaymentMethod,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            DepartmentName = requisition.Department?.Name ?? string.Empty,
            ProjectName = requisition.Project?.Name ?? string.Empty,
            Lines = requisition.Details.Select(d => new CashRequisitionPrintLineDto
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
