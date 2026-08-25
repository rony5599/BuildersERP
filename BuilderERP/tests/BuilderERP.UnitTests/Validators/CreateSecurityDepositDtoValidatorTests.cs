using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSecurityDepositDtoValidatorTests
{
    private readonly CreateSecurityDepositDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_contractor_id_is_empty()
    {
        var model = new CreateSecurityDepositDto { ContractorId = 0L, DepositAmount = 1000 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
    }

    [Fact]
    public void Should_have_error_when_deposit_amount_is_negative()
    {
        var model = new CreateSecurityDepositDto { ContractorId = 1L, DepositAmount = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DepositAmount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSecurityDepositDto
        {
            ContractorId = 1L,
            DepositAmount = 1000,
            DepositDate = DateTime.UtcNow,
            Status = SecurityDepositStatus.Held
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
