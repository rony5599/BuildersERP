using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateQualityChecklistDtoValidator : AbstractValidator<UpdateQualityChecklistDto>
{
    public UpdateQualityChecklistDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ChecklistName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).MaximumLength(100);
        RuleFor(x => x.CheckedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
    }
}
