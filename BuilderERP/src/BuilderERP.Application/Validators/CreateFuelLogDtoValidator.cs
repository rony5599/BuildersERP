using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateFuelLogDtoValidator : AbstractValidator<CreateFuelLogDto>
{
    public CreateFuelLogDtoValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.FuelQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FuelCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
