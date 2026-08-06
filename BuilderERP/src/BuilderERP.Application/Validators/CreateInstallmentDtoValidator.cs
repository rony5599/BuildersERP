using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateInstallmentDtoValidator : AbstractValidator<CreateInstallmentDto>
{
    public CreateInstallmentDtoValidator()
    {
        RuleFor(x => x.InstallmentNumber).GreaterThan(0);
        RuleFor(x => x.DueAmount).GreaterThan(0);
        RuleFor(x => x.PenaltyAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InstallmentPlanId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
