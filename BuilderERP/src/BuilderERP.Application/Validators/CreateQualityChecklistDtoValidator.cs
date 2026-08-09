using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateQualityChecklistDtoValidator : AbstractValidator<CreateQualityChecklistDto>
{
    public CreateQualityChecklistDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ChecklistName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).MaximumLength(100);
        RuleFor(x => x.CheckedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
    }
}
