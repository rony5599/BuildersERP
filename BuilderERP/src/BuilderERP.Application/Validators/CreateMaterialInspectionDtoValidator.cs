using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateMaterialInspectionDtoValidator : AbstractValidator<CreateMaterialInspectionDto>
{
    public CreateMaterialInspectionDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.InspectedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Result).IsInEnum();
    }
}
