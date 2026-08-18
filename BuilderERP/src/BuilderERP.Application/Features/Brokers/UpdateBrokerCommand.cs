using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Brokers;

public record UpdateBrokerCommand(UpdateBrokerDto Dto) : IRequest<bool>;

public class UpdateBrokerCommandHandler : IRequestHandler<UpdateBrokerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrokerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBrokerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Broker>();
        var broker = await repository.GetByIdAsync(request.Dto.Id);
        if (broker is null)
        {
            return false;
        }

        broker.Name = request.Dto.Name;
        broker.Phone = request.Dto.Phone;
        broker.Email = request.Dto.Email;
        broker.Address = request.Dto.Address;
        broker.LicenseNumber = request.Dto.LicenseNumber;
        broker.DefaultCommissionRate = request.Dto.DefaultCommissionRate;

        repository.Update(broker);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
