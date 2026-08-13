using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateFlatHandoverDtoValidator : AbstractValidator<CreateFlatHandoverDto>
{
    public CreateFlatHandoverDtoValidator()
    {
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.HandoverNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.KeyIssuedTo).MaximumLength(150);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
