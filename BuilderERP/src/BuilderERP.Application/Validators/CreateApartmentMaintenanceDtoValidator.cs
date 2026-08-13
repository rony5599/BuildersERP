using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateApartmentMaintenanceDtoValidator : AbstractValidator<CreateApartmentMaintenanceDto>
{
    public CreateApartmentMaintenanceDtoValidator()
    {
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.MaintenanceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MaintenanceType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AssignedTo).MaximumLength(150);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
