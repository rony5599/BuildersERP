using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class SavePoBillDtoValidator : AbstractValidator<SavePoBillDto>
{
    public SavePoBillDtoValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty().WithMessage("Select a purchase order.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.SupplierInvoiceNumber).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.BillDate).When(x => x.DueDate.HasValue)
            .WithMessage("Due date cannot be before the bill date.");
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.PurchaseOrderDetailId).NotEmpty();
            detail.RuleFor(d => d.BilledQuantity).GreaterThan(0);
        });
    }
}
