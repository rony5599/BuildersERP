using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateRiskAssessmentDtoValidator : AbstractValidator<UpdateRiskAssessmentDto>
{
    public UpdateRiskAssessmentDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.AssessedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HazardDescription).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MitigationMeasures).MaximumLength(1000);
        RuleFor(x => x.RiskLevel).IsInEnum();
    }
}
