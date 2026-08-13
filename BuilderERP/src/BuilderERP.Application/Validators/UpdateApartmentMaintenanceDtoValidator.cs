using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateApartmentMaintenanceDtoValidator : AbstractValidator<UpdateApartmentMaintenanceDto>
{
    public UpdateApartmentMaintenanceDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.MaintenanceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MaintenanceType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AssignedTo).MaximumLength(150);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
