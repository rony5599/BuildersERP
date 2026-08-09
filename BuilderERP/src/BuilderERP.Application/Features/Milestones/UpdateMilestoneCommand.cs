using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Milestones;

public record UpdateMilestoneCommand(UpdateMilestoneDto Dto) : IRequest<bool>;

public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Milestone>();
        var milestone = await repository.GetByIdAsync(request.Dto.Id);
        if (milestone is null)
        {
            return false;
        }

        milestone.Name = request.Dto.Name;
        milestone.TargetDate = request.Dto.TargetDate;
        milestone.ActualDate = request.Dto.ActualDate;
        milestone.Status = request.Dto.Status;
        milestone.Remarks = request.Dto.Remarks;
        milestone.ProjectId = request.Dto.ProjectId;

        repository.Update(milestone);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
