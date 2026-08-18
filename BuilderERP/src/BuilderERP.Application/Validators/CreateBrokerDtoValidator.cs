using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateBrokerDtoValidator : AbstractValidator<CreateBrokerDto>
{
    public CreateBrokerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.LicenseNumber).MaximumLength(100);
        RuleFor(x => x.DefaultCommissionRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
    }
}
