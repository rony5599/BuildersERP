using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateCashDisbursementDtoValidator : AbstractValidator<CreateCashDisbursementDto>
{
    public CreateCashDisbursementDtoValidator()
    {
        RuleFor(x => x.CashRequisitionId).NotEmpty().WithMessage("Select a cash requisition.");
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
