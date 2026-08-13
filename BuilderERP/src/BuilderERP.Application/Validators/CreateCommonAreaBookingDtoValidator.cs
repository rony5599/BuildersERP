using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateCommonAreaBookingDtoValidator : AbstractValidator<CreateCommonAreaBookingDto>
{
    public CreateCommonAreaBookingDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.BookingNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FacilityName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
