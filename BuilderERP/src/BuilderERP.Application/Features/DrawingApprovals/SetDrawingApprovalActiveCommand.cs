using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record SetDrawingApprovalActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetDrawingApprovalActiveCommandHandler : IRequestHandler<SetDrawingApprovalActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDrawingApprovalActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDrawingApprovalActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DrawingApproval>();
        var approval = await repository.GetByIdAsync(request.Id);
        if (approval is null)
        {
            return false;
        }

        approval.IsActive = request.IsActive;
        repository.Update(approval);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
