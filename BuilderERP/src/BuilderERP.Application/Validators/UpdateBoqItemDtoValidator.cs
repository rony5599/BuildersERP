using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateBoqItemDtoValidator : AbstractValidator<UpdateBoqItemDto>
{
    public UpdateBoqItemDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.UnitOfMeasure).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BoqId).NotEmpty();
        RuleFor(x => x.WorkGroupId).NotEmpty();
    }
}
