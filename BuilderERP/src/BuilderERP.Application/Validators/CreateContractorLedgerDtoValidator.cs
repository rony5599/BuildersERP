using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateContractorLedgerDtoValidator : AbstractValidator<CreateContractorLedgerDto>
{
    public CreateContractorLedgerDtoValidator()
    {
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DebitAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CreditAmount).GreaterThanOrEqualTo(0);
    }
}
