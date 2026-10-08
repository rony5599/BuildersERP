using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record SetBoqStatusCommand(long Id, BoqStatus ExpectedStatus, BoqStatus NewStatus,
    string? ModifiedBy, string? RejectionReason = null) : IRequest<bool>;

public class SetBoqStatusCommandHandler : IRequestHandler<SetBoqStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetBoqStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(SetBoqStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BoqHeader>();
        var boq = await repository.GetByIdAsync(request.Id);
        if (boq is null || boq.Status != request.ExpectedStatus) return false;

        boq.Status = request.NewStatus;
        boq.ModifiedAt = DateTime.UtcNow;
        boq.ModifiedBy = request.ModifiedBy;
        if (request.NewStatus == BoqStatus.Draft && !string.IsNullOrWhiteSpace(request.RejectionReason))
        {
            boq.RejectionReason = request.RejectionReason.Trim();
            boq.RejectedBy = request.ModifiedBy;
            boq.RejectedAt = DateTime.UtcNow;
            boq.ApprovedBy = null;
            boq.ApprovedAt = null;
        }
        else if (request.NewStatus == BoqStatus.Approved)
        {
            boq.ApprovedBy = request.ModifiedBy;
            boq.ApprovedAt = DateTime.UtcNow;
            boq.RejectionReason = null;
            boq.RejectedBy = null;
            boq.RejectedAt = null;
        }
        repository.Update(boq);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
