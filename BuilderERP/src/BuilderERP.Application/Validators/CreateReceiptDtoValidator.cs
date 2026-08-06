using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateReceiptDtoValidator : AbstractValidator<CreateReceiptDto>
{
    public CreateReceiptDtoValidator()
    {
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AmountPaid).GreaterThan(0);
        RuleFor(x => x.InstallmentId).NotEmpty();
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}
