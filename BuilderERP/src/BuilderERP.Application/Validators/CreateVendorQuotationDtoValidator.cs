using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateVendorQuotationDtoValidator : AbstractValidator<CreateVendorQuotationDto>
{
    public CreateVendorQuotationDtoValidator()
    {
        RuleFor(x => x.QuotationNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.QuotedAmount).GreaterThan(0);
        RuleFor(x => x.DeliveryDays).GreaterThan(0);
        RuleFor(x => x.RfqId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
