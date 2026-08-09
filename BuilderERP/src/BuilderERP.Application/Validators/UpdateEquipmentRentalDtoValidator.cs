using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateEquipmentRentalDtoValidator : AbstractValidator<UpdateEquipmentRentalDto>
{
    public UpdateEquipmentRentalDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.RatePerDay).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
