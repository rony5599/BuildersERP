using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateFollowUpDtoValidator : AbstractValidator<CreateFollowUpDto>
{
    public CreateFollowUpDtoValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
    }
}
