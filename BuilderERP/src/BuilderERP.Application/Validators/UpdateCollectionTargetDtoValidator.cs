using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateCollectionTargetDtoValidator : AbstractValidator<UpdateCollectionTargetDto>
{
    public UpdateCollectionTargetDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.TargetAmount).GreaterThan(0);
        RuleFor(x => x.CollectionOfficerId).NotEmpty();
    }
}
