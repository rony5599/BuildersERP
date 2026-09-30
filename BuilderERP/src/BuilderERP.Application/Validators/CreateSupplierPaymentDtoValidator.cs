using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSupplierPaymentDtoValidator : AbstractValidator<CreateSupplierPaymentDto>
{
    public CreateSupplierPaymentDtoValidator()
    {
        RuleFor(x => x).Must(x => (x.PoBillId is > 0) != (x.EwoBillId is > 0)).WithName("Bill").WithMessage("Select a bill.");
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
