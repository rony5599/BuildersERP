using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Materials;

public record UpdateMaterialCommand(UpdateMaterialDto Dto) : IRequest<bool>;

public class UpdateMaterialCommandHandler : IRequestHandler<UpdateMaterialCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMaterialCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Material>();
        var material = await repository.GetByIdAsync(request.Dto.Id);
        if (material is null)
        {
            return false;
        }

        material.MaterialCode = request.Dto.MaterialCode;
        material.Name = request.Dto.Name;
        material.Description = request.Dto.Description;
        material.UnitOfMeasure = request.Dto.UnitOfMeasure;
        material.ReorderLevel = request.Dto.ReorderLevel;
        material.Barcode = request.Dto.Barcode;

        repository.Update(material);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
