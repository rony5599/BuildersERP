using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdatePurchaseOrderDtoValidator : AbstractValidator<UpdatePurchaseOrderDto>
{
    public UpdatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PONumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.DeliveryDate).NotEmpty();
        RuleFor(x => x.VendorQuotationId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
