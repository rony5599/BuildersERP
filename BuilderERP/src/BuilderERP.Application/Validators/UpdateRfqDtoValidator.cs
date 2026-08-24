using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateRfqDtoValidator : AbstractValidator<UpdateRfqDto>
{
    public UpdateRfqDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RfqNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ClosingDate).NotEmpty();
        RuleFor(x => x.PurchaseRequisitionId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.SupplierIds).NotEmpty().WithMessage("At least one vendor must be invited.");
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.Quantity).GreaterThan(0);
            detail.RuleFor(d => d.UnitOfMeasure).IsInEnum();
        });
    }
}
