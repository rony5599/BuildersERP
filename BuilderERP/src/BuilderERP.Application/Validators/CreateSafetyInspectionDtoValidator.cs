using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSafetyInspectionDtoValidator : AbstractValidator<CreateSafetyInspectionDto>
{
    public CreateSafetyInspectionDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.InspectedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
    }
}
