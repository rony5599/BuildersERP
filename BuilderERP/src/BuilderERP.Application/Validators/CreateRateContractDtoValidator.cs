using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateRateContractDtoValidator : AbstractValidator<CreateRateContractDto>
{
    public CreateRateContractDtoValidator()
    {
        RuleFor(x => x.ContractNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ItemDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.UnitOfMeasure).IsInEnum();
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
    }
}
