using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateRfqDtoValidator : AbstractValidator<CreateRfqDto>
{
    public CreateRfqDtoValidator()
    {
        RuleFor(x => x.RfqNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ClosingDate).NotEmpty();
        RuleFor(x => x.PurchaseRequisitionId).NotEmpty();
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
