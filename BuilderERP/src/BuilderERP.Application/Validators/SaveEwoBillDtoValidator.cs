using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class SaveEwoBillDtoValidator : AbstractValidator<SaveEwoBillDto>
{
    public SaveEwoBillDtoValidator()
    {
        RuleFor(x => x.EngineerWorkOrderId).NotEmpty().WithMessage("Select an engineer work order.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ContractorBillNumber).MaximumLength(100);
        RuleFor(x => x.MrrNumber).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.Details).NotEmpty().WithMessage("Enter the measured quantity of at least one line.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.EngineerWorkOrderDetailId).NotEmpty();
            detail.RuleFor(d => d.MeasuredQuantity).GreaterThanOrEqualTo(0);
        });
        RuleForEach(x => x.Heads).ChildRules(head =>
        {
            head.RuleFor(h => h.EngineerWorkOrderPaymentHeadId).NotEmpty();
            head.RuleFor(h => h.ClaimPercent).InclusiveBetween(0, 100);
        });
        RuleForEach(x => x.Adjustments).ChildRules(adjustment =>
        {
            adjustment.RuleFor(a => a.Type).IsInEnum();
            adjustment.RuleFor(a => a.Description).NotEmpty().WithMessage("Each addition/deduction needs a description.").MaximumLength(200);
            adjustment.RuleFor(a => a.Amount).GreaterThan(0).WithMessage("Each addition/deduction amount must be greater than 0.");
        });
    }
}
