using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateBoqDtoValidator : AbstractValidator<CreateBoqDto>
{
    public CreateBoqDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.BoqName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.VersionNumber).GreaterThan(0);
        RuleFor(x => x.ContingencyPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Add at least one BOQ item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.WorkGroupId).NotEmpty();
            item.RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            item.RuleFor(x => x.UnitOfMeasure).IsInEnum();
            item.RuleFor(x => x.Quantity).GreaterThan(0);
            item.RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        });
    }
}
