using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePpeTrackingDtoValidator : AbstractValidator<CreatePpeTrackingDto>
{
    public CreatePpeTrackingDtoValidator()
    {
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.PpeType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
