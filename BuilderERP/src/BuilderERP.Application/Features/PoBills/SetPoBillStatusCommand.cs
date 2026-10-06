using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PoBills;

public record SetPoBillStatusCommand(long Id, PoBillStatus ExpectedStatus, PoBillStatus NewStatus, string? ModifiedBy, string? RejectionReason = null)
    : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["SupplierPayments", "SupplierLedger"];
}

public class SetPoBillStatusCommandHandler : IRequestHandler<SetPoBillStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetPoBillStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<bool> Handle(SetPoBillStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PoBill>();
        var bill = await repository.GetByIdAsync(request.Id);
        if (bill is null || bill.Status != request.ExpectedStatus) return false;
        bill.Status = request.NewStatus;
        bill.ModifiedAt = DateTime.UtcNow;
        bill.ModifiedBy = request.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(request.RejectionReason))
        {
            bill.RejectionReason = request.RejectionReason.Trim();
            bill.RejectedBy = request.ModifiedBy;
            bill.RejectedAt = DateTime.UtcNow;
        }
        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
