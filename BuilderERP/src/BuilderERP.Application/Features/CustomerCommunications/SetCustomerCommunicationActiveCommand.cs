using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record SetCustomerCommunicationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetCustomerCommunicationActiveCommandHandler : IRequestHandler<SetCustomerCommunicationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCustomerCommunicationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCustomerCommunicationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CustomerCommunication>();
        var communication = await repository.GetByIdAsync(request.Id);
        if (communication is null)
        {
            return false;
        }

        communication.IsActive = request.IsActive;
        repository.Update(communication);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
