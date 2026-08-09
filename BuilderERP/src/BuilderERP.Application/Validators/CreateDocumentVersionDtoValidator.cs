using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateDocumentVersionDtoValidator : AbstractValidator<CreateDocumentVersionDto>
{
    public CreateDocumentVersionDtoValidator()
    {
        RuleFor(x => x.VersionNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ChangeNotes).MaximumLength(1000);
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}
