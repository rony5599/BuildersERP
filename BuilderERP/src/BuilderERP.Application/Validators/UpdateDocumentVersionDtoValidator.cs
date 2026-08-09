using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDocumentVersionDtoValidator : AbstractValidator<UpdateDocumentVersionDto>
{
    public UpdateDocumentVersionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.VersionNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ChangeNotes).MaximumLength(1000);
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}
