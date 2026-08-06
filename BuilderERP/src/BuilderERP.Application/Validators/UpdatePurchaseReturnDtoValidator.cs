using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdatePurchaseReturnDtoValidator : AbstractValidator<UpdatePurchaseReturnDto>
{
    public UpdatePurchaseReturnDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ReturnNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ReturnAmount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.GoodsReceiveId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
