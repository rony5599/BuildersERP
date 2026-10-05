using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EwoBills;

public record CreateEwoBillCommand(SaveEwoBillDto Dto) : IRequest<EwoBillBuildResult>, IInvalidatesFeatures
{
    // Approved bills are credits in the supplier ledger and appear in the payable bill list (SupplierPayments).
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["SupplierPayments"];
}

public class CreateEwoBillCommandHandler : IRequestHandler<CreateEwoBillCommand, EwoBillBuildResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateEwoBillCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<EwoBillBuildResult> Handle(CreateEwoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var built = await EwoBillBuilder.BuildAsync(_unitOfWork, dto, null, cancellationToken);
        if (built.Result != EwoBillBuildResult.Success)
        {
            return built.Result;
        }

        var bill = built.Values!;
        bill.BillNumber = await _numberGenerator.GenerateAsync(built.ProjectId, "EBIL", cancellationToken);
        bill.BillDate = dto.BillDate;
        bill.ContractorBillNumber = dto.ContractorBillNumber;
        bill.MrrNumber = dto.MrrNumber;
        bill.Remarks = dto.Remarks;
        bill.Status = dto.Status;
        foreach (var detail in built.Details!)
        {
            bill.Details.Add(detail);
        }

        foreach (var head in built.Heads!)
        {
            bill.Heads.Add(head);
        }

        foreach (var adjustment in built.Adjustments!)
        {
            bill.Adjustments.Add(adjustment);
        }

        await _unitOfWork.Repository<EwoBill>().AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();
        return EwoBillBuildResult.Success;
    }
}

public enum UpdateEwoBillResult
{
    Success,
    NotFound,
    Locked,
    BuildFailed
}

public record UpdateEwoBillCommand(SaveEwoBillDto Dto) : IRequest<(UpdateEwoBillResult Result, EwoBillBuildResult BuildResult)>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["SupplierPayments"];
}

public class UpdateEwoBillCommandHandler : IRequestHandler<UpdateEwoBillCommand, (UpdateEwoBillResult Result, EwoBillBuildResult BuildResult)>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEwoBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(UpdateEwoBillResult Result, EwoBillBuildResult BuildResult)> Handle(UpdateEwoBillCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var repository = _unitOfWork.Repository<EwoBill>();
        var bill = await repository.Query()
            .Include(b => b.Details)
            .Include(b => b.Heads)
            .Include(b => b.Adjustments)
            .FirstOrDefaultAsync(b => b.Id == dto.Id, cancellationToken);

        if (bill is null)
        {
            return (UpdateEwoBillResult.NotFound, EwoBillBuildResult.Success);
        }

        if (bill.Status != PoBillStatus.Draft)
        {
            return (UpdateEwoBillResult.Locked, EwoBillBuildResult.Success);
        }

        // The work order of an existing bill is fixed; only its measurement, heads and header can change.
        dto.EngineerWorkOrderId = bill.EngineerWorkOrderId;
        var built = await EwoBillBuilder.BuildAsync(_unitOfWork, dto, bill.Id, cancellationToken);
        if (built.Result != EwoBillBuildResult.Success)
        {
            return (UpdateEwoBillResult.BuildFailed, built.Result);
        }

        var detailRepository = _unitOfWork.Repository<EwoBillDetail>();
        foreach (var old in bill.Details.ToList())
        {
            detailRepository.Remove(old);
        }

        var headRepository = _unitOfWork.Repository<EwoBillHead>();
        foreach (var old in bill.Heads.ToList())
        {
            headRepository.Remove(old);
        }

        var adjustmentRepository = _unitOfWork.Repository<EwoBillAdjustment>();
        foreach (var old in bill.Adjustments.ToList())
        {
            adjustmentRepository.Remove(old);
        }

        bill.Details.Clear();
        bill.Heads.Clear();
        bill.Adjustments.Clear();
        foreach (var detail in built.Details!)
        {
            detail.EwoBillId = bill.Id;
            await detailRepository.AddAsync(detail);
        }

        foreach (var head in built.Heads!)
        {
            head.EwoBillId = bill.Id;
            await headRepository.AddAsync(head);
        }

        foreach (var adjustment in built.Adjustments!)
        {
            adjustment.EwoBillId = bill.Id;
            await adjustmentRepository.AddAsync(adjustment);
        }

        var values = built.Values!;
        bill.BillDate = dto.BillDate;
        bill.ContractorBillNumber = dto.ContractorBillNumber;
        bill.MrrNumber = dto.MrrNumber;
        bill.Remarks = dto.Remarks;
        bill.Status = dto.Status;
        bill.MeasuredAmount = values.MeasuredAmount;
        bill.CumulativePercent = values.CumulativePercent;
        bill.CumulativeDue = values.CumulativeDue;
        bill.PreviouslyCertified = values.PreviouslyCertified;
        bill.CertifiedAmount = values.CertifiedAmount;
        bill.AdditionAmount = values.AdditionAmount;
        bill.DeductionAmount = values.DeductionAmount;
        bill.NetPayable = values.NetPayable;
        repository.Update(bill);

        await _unitOfWork.SaveChangesAsync();
        return (UpdateEwoBillResult.Success, EwoBillBuildResult.Success);
    }
}

public enum SetEwoBillActiveResult
{
    Success,
    NotFound,
    LaterBillsExist,
    HasPayments,
    StaleSnapshot
}

public record SetEwoBillActiveCommand(long Id, bool IsActive) : IRequest<SetEwoBillActiveResult>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["SupplierPayments"];
}

// Bills of a work order build on each other cumulatively, so only the latest one can be
// deactivated or restored, and never while it still has active payments.
public class SetEwoBillActiveCommandHandler : IRequestHandler<SetEwoBillActiveCommand, SetEwoBillActiveResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEwoBillActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SetEwoBillActiveResult> Handle(SetEwoBillActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EwoBill>();
        var bill = await repository.GetByIdAsync(request.Id);
        if (bill is null)
        {
            return SetEwoBillActiveResult.NotFound;
        }

        var laterBills = await repository.Query()
            .AnyAsync(b => b.RootWorkOrderId == bill.RootWorkOrderId && b.Id > bill.Id
                           && b.IsActive && b.Status != PoBillStatus.Cancelled, cancellationToken);
        if (laterBills)
        {
            return SetEwoBillActiveResult.LaterBillsExist;
        }

        if (request.IsActive)
        {
            // The bill's cumulative amounts were computed against the earlier bills as they were then;
            // if any of those were deactivated since, the bill must be re-raised instead of restored.
            var prior = await EwoBillBuilder.LoadPriorBillsAsync(_unitOfWork, bill.RootWorkOrderId, bill.Id, cancellationToken);
            if (prior.Certified != bill.PreviouslyCertified || prior.PendingDraftBillNumber is not null)
            {
                return SetEwoBillActiveResult.StaleSnapshot;
            }
        }
        else
        {
            var paid = await _unitOfWork.Repository<SupplierPayment>().Query()
                .AnyAsync(p => p.EwoBillId == bill.Id && p.IsActive, cancellationToken);
            if (paid)
            {
                return SetEwoBillActiveResult.HasPayments;
            }
        }

        bill.IsActive = request.IsActive;
        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return SetEwoBillActiveResult.Success;
    }
}

public record SetEwoBillStatusCommand(long Id, PoBillStatus ExpectedStatus, PoBillStatus NewStatus, string? ModifiedBy)
    : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["SupplierPayments", "SupplierLedger"];
}

public class SetEwoBillStatusCommandHandler : IRequestHandler<SetEwoBillStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetEwoBillStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(SetEwoBillStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EwoBill>();
        var bill = await repository.GetByIdAsync(request.Id);
        if (bill is null || bill.Status != request.ExpectedStatus) return false;
        bill.Status = request.NewStatus;
        bill.ModifiedAt = DateTime.UtcNow;
        bill.ModifiedBy = request.ModifiedBy;
        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
