using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateSiteVisitDtoValidator : AbstractValidator<CreateSiteVisitDto>
{
    public CreateSiteVisitDtoValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Feedback).MaximumLength(1000);
    }
}
