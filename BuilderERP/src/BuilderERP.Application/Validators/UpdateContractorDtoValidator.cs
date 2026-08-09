using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateContractorDtoValidator : AbstractValidator<UpdateContractorDto>
{
    public UpdateContractorDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ContractorCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.ContactPerson).MaximumLength(200);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.LicenseNumber).MaximumLength(100);
        RuleFor(x => x.Specialization).MaximumLength(200);
    }
}
