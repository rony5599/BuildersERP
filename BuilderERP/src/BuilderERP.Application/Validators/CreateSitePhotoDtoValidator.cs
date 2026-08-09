using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSitePhotoDtoValidator : AbstractValidator<CreateSitePhotoDto>
{
    public CreateSitePhotoDtoValidator()
    {
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Caption).MaximumLength(300);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
