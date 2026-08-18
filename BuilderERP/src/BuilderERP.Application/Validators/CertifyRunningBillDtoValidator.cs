using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CertifyRunningBillDtoValidator : AbstractValidator<CertifyRunningBillDto>
{
    public CertifyRunningBillDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CertificateNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CertifiedBy).NotEmpty().MaximumLength(200);
    }
}
