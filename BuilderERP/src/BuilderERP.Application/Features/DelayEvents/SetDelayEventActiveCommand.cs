using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DelayEvents;

public record SetDelayEventActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetDelayEventActiveCommandHandler : IRequestHandler<SetDelayEventActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDelayEventActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDelayEventActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DelayEvent>();
        var delayEvent = await repository.GetByIdAsync(request.Id);
        if (delayEvent is null)
        {
            return false;
        }

        delayEvent.IsActive = request.IsActive;
        repository.Update(delayEvent);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
