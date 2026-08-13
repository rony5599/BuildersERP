using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandMutations;

public record SetLandMutationActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetLandMutationActiveCommandHandler : IRequestHandler<SetLandMutationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLandMutationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLandMutationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandMutation>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
