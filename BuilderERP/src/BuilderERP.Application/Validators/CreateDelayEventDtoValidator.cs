using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateDelayEventDtoValidator : AbstractValidator<CreateDelayEventDto>
{
    public CreateDelayEventDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DelayDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Reason).IsInEnum();
    }
}
