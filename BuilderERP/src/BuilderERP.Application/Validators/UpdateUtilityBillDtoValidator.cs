using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateUtilityBillDtoValidator : AbstractValidator<UpdateUtilityBillDto>
{
    public UpdateUtilityBillDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.BillNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UtilityType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
