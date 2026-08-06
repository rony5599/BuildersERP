using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreatePurchaseRequisitionDtoValidator : AbstractValidator<CreatePurchaseRequisitionDto>
{
    public CreatePurchaseRequisitionDtoValidator()
    {
        RuleFor(x => x.RequisitionNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.RequiredByDate).NotEmpty();
        RuleFor(x => x.EstimatedAmount).GreaterThan(0);
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
