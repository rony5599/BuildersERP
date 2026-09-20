using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetGoodsReceivePrintDataQuery(long Id) : IRequest<GoodsReceivePrintDto?>;

public class GetGoodsReceivePrintDataQueryHandler : IRequestHandler<GetGoodsReceivePrintDataQuery, GoodsReceivePrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGoodsReceivePrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GoodsReceivePrintDto?> Handle(GetGoodsReceivePrintDataQuery request, CancellationToken cancellationToken)
    {
        var receive = await _unitOfWork.Repository<GoodsReceive>().Query()
            .Include(g => g.PurchaseOrder).ThenInclude(o => o!.VendorQuotation).ThenInclude(v => v.Supplier)
            .Include(g => g.PurchaseOrder).ThenInclude(o => o!.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(g => g.EngineerWorkOrder).ThenInclude(o => o!.Supplier)
            .Include(g => g.EngineerWorkOrder).ThenInclude(o => o!.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(g => g.CashPurchaseOrder).ThenInclude(o => o!.Supplier)
            .Include(g => g.CashPurchaseOrder).ThenInclude(o => o!.CashRequisition).ThenInclude(r => r.Project)
            .Include(g => g.Warehouse)
            .Include(g => g.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (receive is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        string sourceDocumentNumber;
        string supplierName;
        string projectName;

        if (receive.SourceType == GrnSourceType.PurchaseOrder)
        {
            sourceDocumentNumber = receive.PurchaseOrder?.PONumber ?? string.Empty;
            supplierName = receive.PurchaseOrder?.VendorQuotation?.Supplier?.Name ?? string.Empty;
            projectName = receive.PurchaseOrder?.VendorQuotation?.Rfq?.PurchaseRequisition?.Project?.Name ?? string.Empty;
        }
        else if (receive.SourceType == GrnSourceType.EngineerWorkOrder)
        {
            sourceDocumentNumber = receive.EngineerWorkOrder?.WorkOrderNo ?? string.Empty;
            supplierName = receive.EngineerWorkOrder?.Supplier?.Name ?? string.Empty;
            projectName = receive.EngineerWorkOrder?.EngineerWorkOrderRequisition?.Project?.Name ?? string.Empty;
        }
        else
        {
            sourceDocumentNumber = receive.CashPurchaseOrder?.CPONumber ?? string.Empty;
            supplierName = receive.CashPurchaseOrder?.Supplier?.Name ?? string.Empty;
            projectName = receive.CashPurchaseOrder?.CashRequisition?.Project?.Name ?? string.Empty;
        }

        return new GoodsReceivePrintDto
        {
            GrnNumber = receive.GrnNumber,
            ReceivedDate = receive.ReceivedDate,
            Status = receive.Status,
            Remarks = receive.Remarks,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            WarehouseName = receive.Warehouse?.Name ?? string.Empty,
            ProjectName = projectName,
            SourceType = receive.SourceType,
            SourceDocumentNumber = sourceDocumentNumber,
            SupplierName = supplierName,
            Lines = receive.Details.Select(d => new GoodsReceivePrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.ReceivedQuantity,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Rate = d.UnitPrice,
                Amount = d.LineTotal,
                BatchNo = d.BatchNo,
                SerialNo = d.SerialNo
            }).ToList()
        };
    }
}
