using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateReceiptDtoValidator : AbstractValidator<UpdateReceiptDto>
{
    public UpdateReceiptDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AmountPaid).GreaterThan(0);
        RuleFor(x => x.InstallmentId).NotEmpty();
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}
