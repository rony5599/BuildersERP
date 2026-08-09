using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDrawingRevisionDtoValidator : AbstractValidator<UpdateDrawingRevisionDto>
{
    public UpdateDrawingRevisionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RevisionCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ChangeDescription).MaximumLength(1000);
        RuleFor(x => x.DrawingId).NotEmpty();
    }
}
