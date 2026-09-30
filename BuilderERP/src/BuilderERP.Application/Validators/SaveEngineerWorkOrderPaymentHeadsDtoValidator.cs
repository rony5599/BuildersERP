using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class SaveEngineerWorkOrderPaymentHeadsDtoValidator : AbstractValidator<SaveEngineerWorkOrderPaymentHeadsDto>
{
    public SaveEngineerWorkOrderPaymentHeadsDtoValidator()
    {
        RuleFor(x => x.EngineerWorkOrderId).NotEmpty();
        RuleFor(x => x.PaymentHeads).ValidPaymentHeads();
    }
}
