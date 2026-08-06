using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PropertyUnits;

public record SetPropertyUnitActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetPropertyUnitActiveCommandHandler : IRequestHandler<SetPropertyUnitActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPropertyUnitActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPropertyUnitActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PropertyUnit>();
        var unit = await repository.GetByIdAsync(request.Id);
        if (unit is null)
        {
            return false;
        }

        unit.IsActive = request.IsActive;
        repository.Update(unit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
