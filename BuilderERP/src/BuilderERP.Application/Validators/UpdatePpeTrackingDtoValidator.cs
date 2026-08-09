using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdatePpeTrackingDtoValidator : AbstractValidator<UpdatePpeTrackingDto>
{
    public UpdatePpeTrackingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.PpeType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
