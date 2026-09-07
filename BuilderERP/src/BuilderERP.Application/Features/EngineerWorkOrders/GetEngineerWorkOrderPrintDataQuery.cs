using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record GetEngineerWorkOrderPrintDataQuery(long Id) : IRequest<EngineerWorkOrderPrintDto?>;

public class GetEngineerWorkOrderPrintDataQueryHandler : IRequestHandler<GetEngineerWorkOrderPrintDataQuery, EngineerWorkOrderPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEngineerWorkOrderPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EngineerWorkOrderPrintDto?> Handle(GetEngineerWorkOrderPrintDataQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.EngineerWorkOrderRequisition)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (workOrder is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var supplier = workOrder.Supplier;

        return new EngineerWorkOrderPrintDto
        {
            WorkOrderNo = workOrder.WorkOrderNo,
            RequisitionNumber = workOrder.EngineerWorkOrderRequisition?.RequisitionNumber ?? string.Empty,
            RevisionNo = workOrder.RevisionNo,
            OrderDate = workOrder.RevisionDate ?? workOrder.CreatedAt,
            Status = workOrder.Status,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            SupplierName = supplier?.Name ?? string.Empty,
            SupplierAddress = supplier?.Address,
            SupplierPhone = supplier?.Phone,
            SupplierEmail = supplier?.Email,
            TermsAndCondition = workOrder.TermsAndCondition,
            Lines = workOrder.Details.Select(d => new EngineerWorkOrderPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Quantity = d.Qty,
                Rate = d.Rate,
                Amount = d.Amount,
                Remarks = d.Remarks
            }).ToList()
        };
    }
}
