using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateFloorDtoValidator : AbstractValidator<UpdateFloorDto>
{
    public UpdateFloorDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TowerId).NotEmpty();
    }
}
