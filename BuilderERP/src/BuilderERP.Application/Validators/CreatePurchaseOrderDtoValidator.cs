using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePurchaseOrderDtoValidator : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.PONumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.DeliveryDate).NotEmpty();
        RuleFor(x => x.VendorQuotationId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
