using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateRunningBillDtoValidator : AbstractValidator<CreateRunningBillDto>
{
    public CreateRunningBillDtoValidator()
    {
        RuleFor(x => x.BillNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.WorkDoneAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PreviousBillAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DeductionAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
