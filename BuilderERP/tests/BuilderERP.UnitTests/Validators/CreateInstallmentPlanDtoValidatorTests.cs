using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateInstallmentPlanDtoValidatorTests
{
    private readonly CreateInstallmentPlanDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_number_of_installments_is_zero()
    {
        var model = new CreateInstallmentPlanDto { TotalAmount = 250000, NumberOfInstallments = 0, SaleAgreementId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.NumberOfInstallments);
    }

    [Fact]
    public void Should_have_error_when_interest_rate_is_negative()
    {
        var model = new CreateInstallmentPlanDto { TotalAmount = 250000, NumberOfInstallments = 10, SaleAgreementId = Guid.NewGuid(), InterestRatePercent = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.InterestRatePercent);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateInstallmentPlanDto { TotalAmount = 250000, NumberOfInstallments = 10, SaleAgreementId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
