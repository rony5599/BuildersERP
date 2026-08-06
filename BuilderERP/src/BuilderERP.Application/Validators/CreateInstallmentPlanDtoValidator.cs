using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateInstallmentPlanDtoValidator : AbstractValidator<CreateInstallmentPlanDto>
{
    public CreateInstallmentPlanDtoValidator()
    {
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.NumberOfInstallments).GreaterThan(0).LessThanOrEqualTo(360);
        RuleFor(x => x.InterestRatePercent).GreaterThanOrEqualTo(0).When(x => x.InterestRatePercent.HasValue);
        RuleFor(x => x.SaleAgreementId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
