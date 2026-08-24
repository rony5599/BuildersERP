using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateGoodsReceiveDtoValidator : AbstractValidator<UpdateGoodsReceiveDto>
{
    public UpdateGoodsReceiveDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.GrnNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.PurchaseOrderDetailId).NotEmpty();
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.ReceivedQuantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitPrice).GreaterThanOrEqualTo(0);
            detail.RuleFor(d => d.VatPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.TaxPercent).InclusiveBetween(0, 100);
        });
    }
}
