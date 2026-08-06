using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateFollowUpDtoValidator : AbstractValidator<UpdateFollowUpDto>
{
    public UpdateFollowUpDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LeadId).NotEmpty();
    }
}
