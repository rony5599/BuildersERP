using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.BookingAmount).GreaterThan(0);
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CancellationReason).MaximumLength(500);
        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required when the booking is cancelled.")
            .When(x => x.Status == BuilderERP.Domain.Enums.BookingRequestStatus.Cancelled);
    }
}
