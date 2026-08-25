using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FollowUps;

public record SetFollowUpActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetFollowUpActiveCommandHandler : IRequestHandler<SetFollowUpActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetFollowUpActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetFollowUpActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FollowUp>();
        var followUp = await repository.GetByIdAsync(request.Id);
        if (followUp is null)
        {
            return false;
        }

        followUp.IsActive = request.IsActive;
        repository.Update(followUp);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
