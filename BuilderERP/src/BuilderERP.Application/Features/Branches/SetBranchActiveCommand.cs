using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Branches;

public record SetBranchActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetBranchActiveCommandHandler : IRequestHandler<SetBranchActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBranchActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBranchActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Branch>();
        var branch = await repository.GetByIdAsync(request.Id);
        if (branch is null)
        {
            return false;
        }

        branch.IsActive = request.IsActive;
        repository.Update(branch);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
