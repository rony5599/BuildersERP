using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateCommissionDtoValidator : AbstractValidator<CreateCommissionDto>
{
    public CreateCommissionDtoValidator()
    {
        RuleFor(x => x.CommissionRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        RuleFor(x => x.CommissionAmount).GreaterThanOrEqualTo(0).When(x => x.CommissionAmount.HasValue);
        RuleFor(x => x.BrokerId).NotEmpty();
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
