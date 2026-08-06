using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Buildings;

public record UpdateBuildingCommand(UpdateBuildingDto Dto) : IRequest<bool>;

public class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBuildingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Building>();
        var building = await repository.GetByIdAsync(request.Dto.Id);
        if (building is null)
        {
            return false;
        }

        building.Name = request.Dto.Name;
        building.Code = request.Dto.Code;
        building.TotalFloors = request.Dto.TotalFloors;
        building.ProjectId = request.Dto.ProjectId;

        repository.Update(building);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
