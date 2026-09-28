using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPoBills;

public record CreateCashPoBillCommand(SaveCashPoBillDto Dto) : IRequest<CashPoBillBuildResult>, IInvalidatesFeatures
{
    // Approved bills are credits in the requester ledger, which is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger"];
}

public class CreateCashPoBillCommandHandler : IRequestHandler<CreateCashPoBillCommand, CashPoBillBuildResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateCashPoBillCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<CashPoBillBuildResult> Handle(CreateCashPoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var built = await CashPoBillLineBuilder.BuildAsync(_unitOfWork, dto, null, cancellationToken);
        if (built.Result != CashPoBillBuildResult.Success)
        {
            return built.Result;
        }

        var bill = new CashPoBill
        {
            BillNumber = await _numberGenerator.GenerateAsync(built.ProjectId, "CBIL", cancellationToken),
            BillDate = dto.BillDate,
            MemoNumber = dto.MemoNumber,
            Remarks = dto.Remarks,
            Status = dto.Status,
            CashPurchaseOrderId = dto.CashPurchaseOrderId,
            RequesterEmployeeId = built.RequesterEmployeeId,
            TotalAmount = built.Total
        };
        foreach (var detail in built.Details)
        {
            bill.Details.Add(detail);
        }

        await _unitOfWork.Repository<CashPoBill>().AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();
        return CashPoBillBuildResult.Success;
    }
}

public enum UpdateCashPoBillResult
{
    Success,
    NotFound,
    Locked,
    OrderNotBillable,
    InvalidLine,
    OverBilled
}

public record UpdateCashPoBillCommand(SaveCashPoBillDto Dto) : IRequest<UpdateCashPoBillResult>, IInvalidatesFeatures
{
    // Approved bills are credits in the requester ledger, which is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger"];
}

public class UpdateCashPoBillCommandHandler : IRequestHandler<UpdateCashPoBillCommand, UpdateCashPoBillResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCashPoBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCashPoBillResult> Handle(UpdateCashPoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var repository = _unitOfWork.Repository<CashPoBill>();
        var bill = await repository.Query()
            .Include(b => b.Details)
            .FirstOrDefaultAsync(b => b.Id == dto.Id, cancellationToken);

        if (bill is null)
        {
            return UpdateCashPoBillResult.NotFound;
        }

        if (bill.Status != PoBillStatus.Draft)
        {
            return UpdateCashPoBillResult.Locked;
        }

        // The Cash PO of an existing bill is fixed; only its lines/header can change.
        dto.CashPurchaseOrderId = bill.CashPurchaseOrderId;
        var built = await CashPoBillLineBuilder.BuildAsync(_unitOfWork, dto, bill.Id, cancellationToken);
        if (built.Result != CashPoBillBuildResult.Success)
        {
            return built.Result switch
            {
                CashPoBillBuildResult.OrderNotBillable => UpdateCashPoBillResult.OrderNotBillable,
                CashPoBillBuildResult.OverBilled => UpdateCashPoBillResult.OverBilled,
                _ => UpdateCashPoBillResult.InvalidLine
            };
        }

        var detailRepository = _unitOfWork.Repository<CashPoBillDetail>();
        foreach (var old in bill.Details.ToList())
        {
            detailRepository.Remove(old);
        }

        bill.Details.Clear();
        foreach (var detail in built.Details)
        {
            detail.CashPoBillId = bill.Id;
            await detailRepository.AddAsync(detail);
        }

        bill.BillDate = dto.BillDate;
        bill.MemoNumber = dto.MemoNumber;
        bill.Remarks = dto.Remarks;
        bill.Status = dto.Status;
        bill.TotalAmount = built.Total;
        repository.Update(bill);

        await _unitOfWork.SaveChangesAsync();
        return UpdateCashPoBillResult.Success;
    }
}

public record SetCashPoBillActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    // Approved bills are credits in the requester ledger, which is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger"];
}

public class SetCashPoBillActiveCommandHandler : IRequestHandler<SetCashPoBillActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCashPoBillActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCashPoBillActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashPoBill>();
        var bill = await repository.GetByIdAsync(request.Id);
        if (bill is null)
        {
            return false;
        }

        bill.IsActive = request.IsActive;
        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
