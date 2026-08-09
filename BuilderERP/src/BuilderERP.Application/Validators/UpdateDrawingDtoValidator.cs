using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDrawingDtoValidator : AbstractValidator<UpdateDrawingDto>
{
    public UpdateDrawingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DrawingNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Discipline).IsInEnum();
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
