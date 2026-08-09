using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateWbsTaskDtoValidator : AbstractValidator<UpdateWbsTaskDto>
{
    public UpdateWbsTaskDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.PercentComplete).InclusiveBetween(0, 100);
        RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
