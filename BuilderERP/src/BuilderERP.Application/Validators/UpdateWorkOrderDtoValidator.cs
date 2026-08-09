using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateWorkOrderDtoValidator : AbstractValidator<UpdateWorkOrderDto>
{
    public UpdateWorkOrderDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WorkOrderNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ContractorId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
