using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateLegalCaseDtoValidator : AbstractValidator<UpdateLegalCaseDto>
{
    public UpdateLegalCaseDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.CaseNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CaseTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CourtName).MaximumLength(150);
        RuleFor(x => x.CaseType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.OpposingParty).MaximumLength(150);
        RuleFor(x => x.LawyerName).MaximumLength(150);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
