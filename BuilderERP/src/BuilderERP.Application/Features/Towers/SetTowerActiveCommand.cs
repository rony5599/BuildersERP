using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Towers;

public record SetTowerActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetTowerActiveCommandHandler : IRequestHandler<SetTowerActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetTowerActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetTowerActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Tower>();
        var tower = await repository.GetByIdAsync(request.Id);
        if (tower is null)
        {
            return false;
        }

        tower.IsActive = request.IsActive;
        repository.Update(tower);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
