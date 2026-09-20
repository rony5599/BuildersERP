using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record CreatePoBillCommand(SavePoBillDto Dto) : IRequest<PoBillBuildResult>;

public class CreatePoBillCommandHandler : IRequestHandler<CreatePoBillCommand, PoBillBuildResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreatePoBillCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<PoBillBuildResult> Handle(CreatePoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var (result, details, total) = await PoBillLineBuilder.BuildAsync(_unitOfWork, dto, null, cancellationToken);
        if (result != PoBillBuildResult.Success)
        {
            return result;
        }

        var projectId = await _unitOfWork.Repository<PurchaseOrder>().Query()
            .Where(o => o.Id == dto.PurchaseOrderId)
            .Select(o => o.VendorQuotation.Rfq.PurchaseRequisition.ProjectId)
            .SingleAsync(cancellationToken);

        var bill = new PoBill
        {
            BillNumber = await _numberGenerator.GenerateAsync(projectId, "PBIL", cancellationToken),
            BillDate = dto.BillDate,
            DueDate = dto.DueDate,
            SupplierInvoiceNumber = dto.SupplierInvoiceNumber,
            Remarks = dto.Remarks,
            Status = dto.Status,
            PurchaseOrderId = dto.PurchaseOrderId,
            TotalAmount = total
        };
        foreach (var detail in details)
        {
            bill.Details.Add(detail);
        }

        await _unitOfWork.Repository<PoBill>().AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();
        return PoBillBuildResult.Success;
    }
}
