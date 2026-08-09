using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSalaryDtoValidator : AbstractValidator<CreateSalaryDto>
{
    public CreateSalaryDtoValidator()
    {
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodStart);
        RuleFor(x => x.DaysWorked).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BasicAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OvertimeAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DeductionAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
