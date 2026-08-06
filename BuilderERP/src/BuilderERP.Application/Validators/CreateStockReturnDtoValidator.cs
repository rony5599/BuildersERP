using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateStockReturnDtoValidator : AbstractValidator<CreateStockReturnDto>
{
    public CreateStockReturnDtoValidator()
    {
        RuleFor(x => x.ReturnNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
    }
}
