using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record UpdateDrawingApprovalCommand(UpdateDrawingApprovalDto Dto) : IRequest<bool>;

public class UpdateDrawingApprovalCommandHandler : IRequestHandler<UpdateDrawingApprovalCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDrawingApprovalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDrawingApprovalCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DrawingApproval>();
        var entity = await repository.GetByIdAsync(request.Dto.Id);
        if (entity is null)
        {
            return false;
        }

        entity.DrawingId = request.Dto.DrawingId;
        entity.DrawingRevisionId = request.Dto.DrawingRevisionId;
        entity.ApproverName = request.Dto.ApproverName;
        entity.Status = request.Dto.Status;
        entity.Comments = request.Dto.Comments;
        entity.RequestedDate = request.Dto.RequestedDate;
        entity.ActionDate = request.Dto.Status != ApprovalStatus.Pending && entity.ActionDate is null ? DateTime.UtcNow : request.Dto.ActionDate;

        repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
