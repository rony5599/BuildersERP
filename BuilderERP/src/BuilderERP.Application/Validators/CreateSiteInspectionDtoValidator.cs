using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSiteInspectionDtoValidator : AbstractValidator<CreateSiteInspectionDto>
{
    public CreateSiteInspectionDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.InspectedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
    }
}
