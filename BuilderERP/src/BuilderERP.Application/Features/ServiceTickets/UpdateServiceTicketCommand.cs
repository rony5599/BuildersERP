using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ServiceTickets;

public record UpdateServiceTicketCommand(UpdateServiceTicketDto Dto) : IRequest<bool>;

public class UpdateServiceTicketCommandHandler : IRequestHandler<UpdateServiceTicketCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateServiceTicketCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ServiceTicket>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.TicketNumber = request.Dto.TicketNumber;
        item.Subject = request.Dto.Subject;
        item.Description = request.Dto.Description;
        item.Category = request.Dto.Category;
        item.Priority = request.Dto.Priority;
        item.Status = request.Dto.Status;
        item.RaisedDate = request.Dto.RaisedDate;
        item.ClosedDate = request.Dto.ClosedDate;
        item.AssignedTo = request.Dto.AssignedTo;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
