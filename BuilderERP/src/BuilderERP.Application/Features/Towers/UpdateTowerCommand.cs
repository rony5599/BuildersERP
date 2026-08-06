using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Towers;

public record UpdateTowerCommand(UpdateTowerDto Dto) : IRequest<bool>;

public class UpdateTowerCommandHandler : IRequestHandler<UpdateTowerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTowerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateTowerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Tower>();
        var tower = await repository.GetByIdAsync(request.Dto.Id);
        if (tower is null)
        {
            return false;
        }

        tower.Name = request.Dto.Name;
        tower.Code = request.Dto.Code;
        tower.BuildingId = request.Dto.BuildingId;

        repository.Update(tower);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
