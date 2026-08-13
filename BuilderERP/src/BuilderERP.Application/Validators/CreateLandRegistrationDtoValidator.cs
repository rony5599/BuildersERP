using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateLandRegistrationDtoValidator : AbstractValidator<CreateLandRegistrationDto>
{
    public CreateLandRegistrationDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DeedNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SubRegistryOffice).MaximumLength(150);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
