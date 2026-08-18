using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateCustomerCommunicationDtoValidator : AbstractValidator<CreateCustomerCommunicationDto>
{
    public CreateCustomerCommunicationDtoValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
