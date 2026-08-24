using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateMaterialDtoValidator : AbstractValidator<UpdateMaterialDto>
{
    public UpdateMaterialDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.UnitOfMeasure).IsInEnum();
        RuleFor(x => x.PurchaseUnit).IsInEnum();
        RuleFor(x => x.UnitConversionFactor).GreaterThan(0);
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxStockLevel).GreaterThanOrEqualTo(x => x.MinStockLevel);
        RuleFor(x => x.StandardPurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.VatPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.TaxPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.Barcode).MaximumLength(100);
        RuleFor(x => x.Brand).MaximumLength(100);
    }
}
