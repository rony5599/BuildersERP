using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateOvertimeDtoValidator : AbstractValidator<CreateOvertimeDto>
{
    public CreateOvertimeDtoValidator()
    {
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.Hours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RatePerHour).GreaterThanOrEqualTo(0);
    }
}
