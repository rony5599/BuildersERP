using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionTargets;

public record SetCollectionTargetActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetCollectionTargetActiveCommandHandler : IRequestHandler<SetCollectionTargetActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCollectionTargetActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCollectionTargetActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CollectionTarget>();
        var target = await repository.GetByIdAsync(request.Id);
        if (target is null)
        {
            return false;
        }

        target.IsActive = request.IsActive;
        repository.Update(target);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
