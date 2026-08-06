using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PropertyUnits;

public record UpdatePropertyUnitCommand(UpdatePropertyUnitDto Dto) : IRequest<bool>;

public class UpdatePropertyUnitCommandHandler : IRequestHandler<UpdatePropertyUnitCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePropertyUnitCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePropertyUnitCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PropertyUnit>();
        var unit = await repository.GetByIdAsync(request.Dto.Id);
        if (unit is null)
        {
            return false;
        }

        unit.UnitNumber = request.Dto.UnitNumber;
        unit.UnitType = request.Dto.UnitType;
        unit.Area = request.Dto.Area;
        unit.Price = request.Dto.Price;
        unit.BookingStatus = request.Dto.BookingStatus;
        unit.FloorId = request.Dto.FloorId;

        repository.Update(unit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
