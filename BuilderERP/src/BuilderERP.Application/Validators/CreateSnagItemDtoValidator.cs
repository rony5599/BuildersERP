using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSnagItemDtoValidator : AbstractValidator<CreateSnagItemDto>
{
    public CreateSnagItemDtoValidator()
    {
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.SnagNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Location).MaximumLength(150);
        RuleFor(x => x.Severity).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
