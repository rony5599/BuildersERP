using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateVendorQuotationDtoValidator : AbstractValidator<CreateVendorQuotationDto>
{
    public CreateVendorQuotationDtoValidator()
    {
        RuleFor(x => x.QuotationNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DeliveryDays).GreaterThan(0);
        RuleFor(x => x.RfqId).NotEmpty();
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.Quantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitPrice).GreaterThanOrEqualTo(0);
            detail.RuleFor(d => d.DiscountPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.VatPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.TaxPercent).InclusiveBetween(0, 100);
        });
    }
}
