using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateLegalAgreementDtoValidator : AbstractValidator<CreateLegalAgreementDto>
{
    public CreateLegalAgreementDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.AgreementNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AgreementType).IsInEnum();
        RuleFor(x => x.PartyName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
