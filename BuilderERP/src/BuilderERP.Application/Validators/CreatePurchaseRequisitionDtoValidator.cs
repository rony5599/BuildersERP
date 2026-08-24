using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePurchaseRequisitionDtoValidator : AbstractValidator<CreatePurchaseRequisitionDto>
{
    public CreatePurchaseRequisitionDtoValidator()
    {
        RuleFor(x => x.RequiredByDate).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.Quantity).GreaterThan(0);
            detail.RuleFor(d => d.EstimatedUnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
