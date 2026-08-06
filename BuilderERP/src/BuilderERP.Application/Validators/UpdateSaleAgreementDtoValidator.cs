using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSaleAgreementDtoValidator : AbstractValidator<UpdateSaleAgreementDto>
{
    public UpdateSaleAgreementDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AgreementNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TotalSalePrice).GreaterThan(0);
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
