using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class SaveCashPoBillDtoValidator : AbstractValidator<SaveCashPoBillDto>
{
    public SaveCashPoBillDtoValidator()
    {
        RuleFor(x => x.CashPurchaseOrderId).NotEmpty().WithMessage("Select a cash purchase order.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.MemoNumber).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.CashPurchaseOrderDetailId).NotEmpty();
            detail.RuleFor(d => d.BilledQuantity).GreaterThan(0);
        });
    }
}
