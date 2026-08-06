using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateStockTransferDtoValidator : AbstractValidator<CreateStockTransferDto>
{
    public CreateStockTransferDtoValidator()
    {
        RuleFor(x => x.TransferNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.FromWarehouseId).NotEmpty();
        RuleFor(x => x.ToWarehouseId).NotEmpty();
        RuleFor(x => x.ToWarehouseId).NotEqual(x => x.FromWarehouseId).WithMessage("To Warehouse must be different from From Warehouse.");
    }
}
