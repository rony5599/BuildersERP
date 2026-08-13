using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ServiceTickets;

public record SetServiceTicketActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetServiceTicketActiveCommandHandler : IRequestHandler<SetServiceTicketActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetServiceTicketActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetServiceTicketActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ServiceTicket>();
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
