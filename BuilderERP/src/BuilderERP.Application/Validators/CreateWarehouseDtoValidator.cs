using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateWarehouseDtoValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseDtoValidator()
    {
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(500);
        RuleFor(x => x.BranchId).NotEmpty();
    }
}
