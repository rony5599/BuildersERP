using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateMilestoneDtoValidator : AbstractValidator<CreateMilestoneDto>
{
    public CreateMilestoneDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Remarks).MaximumLength(1000);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
