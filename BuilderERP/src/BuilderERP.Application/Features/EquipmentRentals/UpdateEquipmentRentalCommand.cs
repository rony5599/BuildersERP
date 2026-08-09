using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record UpdateEquipmentRentalCommand(UpdateEquipmentRentalDto Dto) : IRequest<bool>;

public class UpdateEquipmentRentalCommandHandler : IRequestHandler<UpdateEquipmentRentalCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEquipmentRentalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateEquipmentRentalCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EquipmentRental>();
        var rental = await repository.GetByIdAsync(request.Dto.Id);
        if (rental is null)
        {
            return false;
        }

        rental.EquipmentId = request.Dto.EquipmentId;
        rental.SupplierId = request.Dto.SupplierId;
        rental.ProjectId = request.Dto.ProjectId;
        rental.RentalStartDate = request.Dto.RentalStartDate;
        rental.RentalEndDate = request.Dto.RentalEndDate;
        rental.RatePerDay = request.Dto.RatePerDay;
        rental.Status = request.Dto.Status;
        rental.TotalAmount = rental.RentalEndDate.HasValue
            ? (decimal)(rental.RentalEndDate.Value - rental.RentalStartDate).TotalDays * rental.RatePerDay
            : 0m;

        repository.Update(rental);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
