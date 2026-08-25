using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandRegistrations;

public record SetLandRegistrationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetLandRegistrationActiveCommandHandler : IRequestHandler<SetLandRegistrationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLandRegistrationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLandRegistrationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandRegistration>();
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
