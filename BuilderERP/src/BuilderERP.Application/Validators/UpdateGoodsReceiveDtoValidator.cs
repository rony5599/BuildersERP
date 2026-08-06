using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateGoodsReceiveDtoValidator : AbstractValidator<UpdateGoodsReceiveDto>
{
    public UpdateGoodsReceiveDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.GrnNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ReceivedAmount).GreaterThan(0);
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
