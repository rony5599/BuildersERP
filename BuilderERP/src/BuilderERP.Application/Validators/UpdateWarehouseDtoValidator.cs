using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDto>
{
    public UpdateWarehouseDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).MaximumLength(500);
        RuleFor(x => x.BranchId).NotEmpty();
    }
}
