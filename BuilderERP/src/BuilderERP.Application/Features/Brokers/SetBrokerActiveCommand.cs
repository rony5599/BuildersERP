using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Brokers;

public record SetBrokerActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetBrokerActiveCommandHandler : IRequestHandler<SetBrokerActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBrokerActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBrokerActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Broker>();
        var broker = await repository.GetByIdAsync(request.Id);
        if (broker is null)
        {
            return false;
        }

        broker.IsActive = request.IsActive;
        repository.Update(broker);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
