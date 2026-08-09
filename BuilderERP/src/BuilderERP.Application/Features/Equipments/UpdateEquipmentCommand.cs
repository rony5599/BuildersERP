using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Equipments;

public record UpdateEquipmentCommand(UpdateEquipmentDto Dto) : IRequest<bool>;

public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEquipmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Equipment>();
        var equipment = await repository.GetByIdAsync(request.Dto.Id);
        if (equipment is null)
        {
            return false;
        }

        equipment.EquipmentCode = request.Dto.EquipmentCode;
        equipment.Name = request.Dto.Name;
        equipment.Type = request.Dto.Type;
        equipment.Model = request.Dto.Model;
        equipment.RegistrationNumber = request.Dto.RegistrationNumber;
        equipment.PurchaseDate = request.Dto.PurchaseDate;
        equipment.Status = request.Dto.Status;

        repository.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
