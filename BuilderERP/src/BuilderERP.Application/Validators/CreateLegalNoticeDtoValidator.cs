using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateLegalNoticeDtoValidator : AbstractValidator<CreateLegalNoticeDto>
{
    public CreateLegalNoticeDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.NoticeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NoticeType).IsInEnum();
        RuleFor(x => x.IssuedTo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
