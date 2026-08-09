using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSafetyAuditDtoValidator : AbstractValidator<UpdateSafetyAuditDto>
{
    public UpdateSafetyAuditDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.AuditedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
        RuleFor(x => x.Findings).MaximumLength(1000);
        RuleFor(x => x.Status).IsInEnum();
    }
}
