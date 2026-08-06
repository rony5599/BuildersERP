using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateStockIssueDtoValidator : AbstractValidator<UpdateStockIssueDto>
{
    public UpdateStockIssueDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.IssueNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.IssuedTo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
    }
}
