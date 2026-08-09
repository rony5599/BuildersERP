using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateDrawingRevisionDtoValidator : AbstractValidator<CreateDrawingRevisionDto>
{
    public CreateDrawingRevisionDtoValidator()
    {
        RuleFor(x => x.RevisionCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ChangeDescription).MaximumLength(1000);
        RuleFor(x => x.DrawingId).NotEmpty();
    }
}
