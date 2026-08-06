using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warehouses;

public record UpdateWarehouseCommand(UpdateWarehouseDto Dto) : IRequest<bool>;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Warehouse>();
        var warehouse = await repository.GetByIdAsync(request.Dto.Id);
        if (warehouse is null)
        {
            return false;
        }

        warehouse.WarehouseCode = request.Dto.WarehouseCode;
        warehouse.Name = request.Dto.Name;
        warehouse.Location = request.Dto.Location;
        warehouse.BranchId = request.Dto.BranchId;

        repository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
