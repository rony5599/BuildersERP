using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateIncidentReportDtoValidator : AbstractValidator<CreateIncidentReportDto>
{
    public CreateIncidentReportDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ReportedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.InjuredPersonName).MaximumLength(200);
        RuleFor(x => x.CorrectiveAction).MaximumLength(1000);
        RuleFor(x => x.Severity).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
