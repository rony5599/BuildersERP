using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateInquiryDtoValidator : AbstractValidator<CreateInquiryDto>
{
    public CreateInquiryDtoValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.LeadId).NotEmpty();
    }
}
