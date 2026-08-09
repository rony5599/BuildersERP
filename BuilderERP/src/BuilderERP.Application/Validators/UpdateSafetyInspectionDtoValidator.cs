using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSafetyInspectionDtoValidator : AbstractValidator<UpdateSafetyInspectionDto>
{
    public UpdateSafetyInspectionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.InspectedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
    }
}
