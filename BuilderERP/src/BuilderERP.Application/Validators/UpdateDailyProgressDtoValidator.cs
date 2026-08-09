using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDailyProgressDtoValidator : AbstractValidator<UpdateDailyProgressDto>
{
    public UpdateDailyProgressDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PercentComplete).InclusiveBetween(0, 100);
        RuleFor(x => x.ManpowerCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Remarks).MaximumLength(1000);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
