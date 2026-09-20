using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record UpdatePoBillCommand(SavePoBillDto Dto) : IRequest<UpdatePoBillResult>;

public enum UpdatePoBillResult
{
    Success,
    NotFound,
    Locked,
    PurchaseOrderNotBillable,
    InvalidLine,
    OverBilled
}

public class UpdatePoBillCommandHandler : IRequestHandler<UpdatePoBillCommand, UpdatePoBillResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePoBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePoBillResult> Handle(UpdatePoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var repository = _unitOfWork.Repository<PoBill>();
        var bill = await repository.Query()
            .Include(b => b.Details)
            .FirstOrDefaultAsync(b => b.Id == dto.Id, cancellationToken);

        if (bill is null)
        {
            return UpdatePoBillResult.NotFound;
        }

        if (bill.Status != PoBillStatus.Draft)
        {
            return UpdatePoBillResult.Locked;
        }

        // The PO of an existing bill is fixed; only its lines/header can change.
        dto.PurchaseOrderId = bill.PurchaseOrderId;
        var (result, details, total) = await PoBillLineBuilder.BuildAsync(_unitOfWork, dto, bill.Id, cancellationToken);
        if (result != PoBillBuildResult.Success)
        {
            return result switch
            {
                PoBillBuildResult.PurchaseOrderNotBillable => UpdatePoBillResult.PurchaseOrderNotBillable,
                PoBillBuildResult.OverBilled => UpdatePoBillResult.OverBilled,
                _ => UpdatePoBillResult.InvalidLine
            };
        }

        var detailRepository = _unitOfWork.Repository<PoBillDetail>();
        foreach (var old in bill.Details.ToList())
        {
            detailRepository.Remove(old);
        }

        bill.Details.Clear();
        foreach (var detail in details)
        {
            detail.PoBillId = bill.Id;
            await detailRepository.AddAsync(detail);
        }

        bill.BillDate = dto.BillDate;
        bill.DueDate = dto.DueDate;
        bill.SupplierInvoiceNumber = dto.SupplierInvoiceNumber;
        bill.Remarks = dto.Remarks;
        bill.Status = dto.Status;
        bill.TotalAmount = total;
        repository.Update(bill);

        await _unitOfWork.SaveChangesAsync();
        return UpdatePoBillResult.Success;
    }
}
