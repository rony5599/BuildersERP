using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSiteVisitDtoValidator : AbstractValidator<UpdateSiteVisitDto>
{
    public UpdateSiteVisitDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Feedback).MaximumLength(1000);
    }
}
