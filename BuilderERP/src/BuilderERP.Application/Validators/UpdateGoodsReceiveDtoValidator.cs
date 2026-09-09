using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Enums;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateGoodsReceiveDtoValidator : AbstractValidator<UpdateGoodsReceiveDto>
{
    public UpdateGoodsReceiveDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.GrnNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SourceType).IsInEnum();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();

        RuleFor(x => x.PurchaseOrderId).NotEmpty()
            .When(x => x.SourceType == GrnSourceType.PurchaseOrder)
            .WithMessage("Purchase order is required.");
        RuleFor(x => x.EngineerWorkOrderId).NotEmpty()
            .When(x => x.SourceType == GrnSourceType.EngineerWorkOrder)
            .WithMessage("Engineer work order is required.");
        RuleFor(x => x.CashPurchaseOrderId).NotEmpty()
            .When(x => x.SourceType == GrnSourceType.CashPurchaseOrder)
            .WithMessage("Cash purchase order is required.");

        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");

        RuleFor(x => x).Must(dto => dto.Details.All(d => d.PurchaseOrderDetailId.HasValue))
            .When(x => x.SourceType == GrnSourceType.PurchaseOrder)
            .WithMessage("Every line item must reference a purchase order line.");
        RuleFor(x => x).Must(dto => dto.Details.All(d => d.EngineerWorkOrderDetailId.HasValue))
            .When(x => x.SourceType == GrnSourceType.EngineerWorkOrder)
            .WithMessage("Every line item must reference an engineer work order line.");
        RuleFor(x => x).Must(dto => dto.Details.All(d => d.CashPurchaseOrderDetailId.HasValue))
            .When(x => x.SourceType == GrnSourceType.CashPurchaseOrder)
            .WithMessage("Every line item must reference a cash purchase order line.");

        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.ReceivedQuantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitPrice).GreaterThanOrEqualTo(0);
            detail.RuleFor(d => d.VatPercent).InclusiveBetween(0, 100);
            detail.RuleFor(d => d.TaxPercent).InclusiveBetween(0, 100);
        });
    }
}
