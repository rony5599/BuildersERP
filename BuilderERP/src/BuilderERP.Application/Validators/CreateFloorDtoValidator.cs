using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateFloorDtoValidator : AbstractValidator<CreateFloorDto>
{
    public CreateFloorDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TowerId).NotEmpty();
    }
}
