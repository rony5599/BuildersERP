using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdatePerformanceEvaluationDtoValidator : AbstractValidator<UpdatePerformanceEvaluationDto>
{
    public UpdatePerformanceEvaluationDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.QualityScore).InclusiveBetween(1, 5);
        RuleFor(x => x.TimelinessScore).InclusiveBetween(1, 5);
        RuleFor(x => x.SafetyScore).InclusiveBetween(1, 5);
    }
}
