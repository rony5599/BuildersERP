using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record SetEquipmentRentalActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetEquipmentRentalActiveCommandHandler : IRequestHandler<SetEquipmentRentalActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEquipmentRentalActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetEquipmentRentalActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EquipmentRental>();
        var rental = await repository.GetByIdAsync(request.Id);
        if (rental is null)
        {
            return false;
        }

        rental.IsActive = request.IsActive;
        repository.Update(rental);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
