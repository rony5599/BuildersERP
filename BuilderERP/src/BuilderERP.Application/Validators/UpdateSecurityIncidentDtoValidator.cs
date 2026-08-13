using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSecurityIncidentDtoValidator : AbstractValidator<UpdateSecurityIncidentDto>
{
    public UpdateSecurityIncidentDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.IncidentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IncidentType).IsInEnum();
        RuleFor(x => x.Location).MaximumLength(150);
        RuleFor(x => x.ReportedBy).MaximumLength(150);
        RuleFor(x => x.Severity).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ActionTaken).MaximumLength(1000);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
