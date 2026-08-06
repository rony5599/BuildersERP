using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdatePropertyUnitDtoValidator : AbstractValidator<UpdatePropertyUnitDto>
{
    public UpdatePropertyUnitDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UnitNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FloorId).NotEmpty();
        RuleFor(x => x.UnitType).IsInEnum();
        RuleFor(x => x.BookingStatus).IsInEnum();
        RuleFor(x => x.Area).GreaterThanOrEqualTo(0).When(x => x.Area.HasValue);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
    }
}
