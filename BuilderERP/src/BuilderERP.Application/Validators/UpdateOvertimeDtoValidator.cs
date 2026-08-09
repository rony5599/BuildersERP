using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateOvertimeDtoValidator : AbstractValidator<UpdateOvertimeDto>
{
    public UpdateOvertimeDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.Hours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RatePerHour).GreaterThanOrEqualTo(0);
    }
}
