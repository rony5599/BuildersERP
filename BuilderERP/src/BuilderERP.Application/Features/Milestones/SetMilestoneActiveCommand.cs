using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Milestones;

public record SetMilestoneActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetMilestoneActiveCommandHandler : IRequestHandler<SetMilestoneActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMilestoneActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetMilestoneActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Milestone>();
        var milestone = await repository.GetByIdAsync(request.Id);
        if (milestone is null)
        {
            return false;
        }

        milestone.IsActive = request.IsActive;
        repository.Update(milestone);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
