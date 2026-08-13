using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateWarrantyDtoValidator : AbstractValidator<CreateWarrantyDto>
{
    public CreateWarrantyDtoValidator()
    {
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.WarrantyNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ItemCovered).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WarrantyType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
