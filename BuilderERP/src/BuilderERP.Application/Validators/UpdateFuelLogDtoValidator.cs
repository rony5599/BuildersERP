using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateFuelLogDtoValidator : AbstractValidator<UpdateFuelLogDto>
{
    public UpdateFuelLogDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.FuelQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FuelCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
