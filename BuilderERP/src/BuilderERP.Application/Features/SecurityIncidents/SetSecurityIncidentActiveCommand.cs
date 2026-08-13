using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record SetSecurityIncidentActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetSecurityIncidentActiveCommandHandler : IRequestHandler<SetSecurityIncidentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSecurityIncidentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSecurityIncidentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SecurityIncident>();
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
