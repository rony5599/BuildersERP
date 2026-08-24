using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePurchaseOrderDtoValidator : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.PONumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DeliveryDate).NotEmpty();
        RuleFor(x => x.VendorQuotationId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.OrderedQuantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitPrice).GreaterThanOrEqualTo(0);
            detail.RuleFor(d => d.DiscountPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.VatPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.TaxPercent).InclusiveBetween(0, 100);
        });
    }
}
