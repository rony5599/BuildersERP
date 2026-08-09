using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSecurityDepositDtoValidator : AbstractValidator<UpdateSecurityDepositDto>
{
    public UpdateSecurityDepositDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
