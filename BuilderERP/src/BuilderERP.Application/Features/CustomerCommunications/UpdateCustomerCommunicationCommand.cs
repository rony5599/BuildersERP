using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record UpdateCustomerCommunicationCommand(UpdateCustomerCommunicationDto Dto) : IRequest<bool>;

public class UpdateCustomerCommunicationCommandHandler : IRequestHandler<UpdateCustomerCommunicationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommunicationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCustomerCommunicationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CustomerCommunication>();
        var communication = await repository.GetByIdAsync(request.Dto.Id);
        if (communication is null)
        {
            return false;
        }

        communication.CommunicationDate = request.Dto.CommunicationDate;
        communication.Type = request.Dto.Type;
        communication.Subject = request.Dto.Subject;
        communication.Notes = request.Dto.Notes;
        communication.CustomerId = request.Dto.CustomerId;

        repository.Update(communication);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
