using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record SetOperatorAssignmentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetOperatorAssignmentActiveCommandHandler : IRequestHandler<SetOperatorAssignmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetOperatorAssignmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetOperatorAssignmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<OperatorAssignment>();
        var assignment = await repository.GetByIdAsync(request.Id);
        if (assignment is null)
        {
            return false;
        }

        assignment.IsActive = request.IsActive;
        repository.Update(assignment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
