using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateEngineerWorkOrderDtoValidator : AbstractValidator<CreateEngineerWorkOrderDto>
{
    public CreateEngineerWorkOrderDtoValidator()
    {
        RuleFor(x => x.EngineerWorkOrderRequisitionId).NotEmpty();
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.Details).ChildRules(detail =>
        {
            detail.RuleFor(d => d.MaterialId).NotEmpty();
            detail.RuleFor(d => d.Qty).GreaterThan(0);
            detail.RuleFor(d => d.Rate).GreaterThanOrEqualTo(0);
        });
    }
}
