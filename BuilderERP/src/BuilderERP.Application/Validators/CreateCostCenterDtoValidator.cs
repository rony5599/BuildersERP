using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateCostCenterDtoValidator : AbstractValidator<CreateCostCenterDto>
{
    public CreateCostCenterDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
