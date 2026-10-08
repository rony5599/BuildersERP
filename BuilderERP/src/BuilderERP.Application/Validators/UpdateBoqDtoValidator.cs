using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateBoqDtoValidator : AbstractValidator<UpdateBoqDto>
{
    public UpdateBoqDtoValidator()
    {
        Include(new CreateBoqDtoValidator());
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
