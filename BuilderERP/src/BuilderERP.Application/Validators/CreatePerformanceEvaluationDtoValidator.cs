using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePerformanceEvaluationDtoValidator : AbstractValidator<CreatePerformanceEvaluationDto>
{
    public CreatePerformanceEvaluationDtoValidator()
    {
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.QualityScore).InclusiveBetween(1, 5);
        RuleFor(x => x.TimelinessScore).InclusiveBetween(1, 5);
        RuleFor(x => x.SafetyScore).InclusiveBetween(1, 5);
    }
}
