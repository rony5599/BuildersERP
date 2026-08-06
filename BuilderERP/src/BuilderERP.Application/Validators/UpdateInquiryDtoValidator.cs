using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateInquiryDtoValidator : AbstractValidator<UpdateInquiryDto>
{
    public UpdateInquiryDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.LeadId).NotEmpty();
    }
}
