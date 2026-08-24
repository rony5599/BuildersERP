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
        material.CategoryId = request.Dto.CategoryId;
        material.Brand = request.Dto.Brand;
        material.PurchaseUnit = request.Dto.PurchaseUnit;
        material.UnitConversionFactor = request.Dto.UnitConversionFactor;
        material.MinStockLevel = request.Dto.MinStockLevel;
        material.MaxStockLevel = request.Dto.MaxStockLevel;
        material.StandardPurchasePrice = request.Dto.StandardPurchasePrice;
        material.VatPercent = request.Dto.VatPercent;
        material.TaxPercent = request.Dto.TaxPercent;
        material.DiscountPercent = request.Dto.DiscountPercent;
        material.IsBatchTracked = request.Dto.IsBatchTracked;
        material.IsSerialTracked = request.Dto.IsSerialTracked;

        repository.Update(material);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
