using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateStockAdjustmentDtoValidator : AbstractValidator<CreateStockAdjustmentDto>
{
    public CreateStockAdjustmentDtoValidator()
    {
        RuleFor(x => x.AdjustmentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.QuantityDelta).NotEqual(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
    }
}
