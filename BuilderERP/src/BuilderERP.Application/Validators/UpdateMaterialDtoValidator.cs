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
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Barcode).MaximumLength(100);
    }
}
