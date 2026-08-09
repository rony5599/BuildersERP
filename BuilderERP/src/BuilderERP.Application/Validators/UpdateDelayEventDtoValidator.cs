using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDelayEventDtoValidator : AbstractValidator<UpdateDelayEventDto>
{
    public UpdateDelayEventDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DelayDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Reason).IsInEnum();
    }
}
