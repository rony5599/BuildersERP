using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSecurityDepositDtoValidator : AbstractValidator<CreateSecurityDepositDto>
{
    public CreateSecurityDepositDtoValidator()
    {
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
