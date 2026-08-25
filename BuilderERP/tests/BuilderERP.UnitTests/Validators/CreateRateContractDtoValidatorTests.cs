using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateRateContractDtoValidatorTests
{
    private readonly CreateRateContractDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_contract_number_is_empty()
    {
        var model = new CreateRateContractDto { ContractNumber = "", ItemDescription = "Cement bags", ContractorId = 1L, Rate = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractNumber);
    }

    [Fact]
    public void Should_have_error_when_item_description_is_empty()
    {
        var model = new CreateRateContractDto { ContractNumber = "RC-001", ItemDescription = "", ContractorId = 1L, Rate = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ItemDescription);
    }

    [Fact]
    public void Should_have_error_when_contractor_id_is_empty()
    {
        var model = new CreateRateContractDto { ContractNumber = "RC-001", ItemDescription = "Cement bags", ContractorId = 0L, Rate = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
    }

    [Fact]
    public void Should_have_error_when_rate_is_negative()
    {
        var model = new CreateRateContractDto { ContractNumber = "RC-001", ItemDescription = "Cement bags", ContractorId = 1L, Rate = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Rate);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateRateContractDto
        {
            ContractNumber = "RC-001",
            ItemDescription = "Cement bags",
            ContractorId = 1L,
            UnitOfMeasure = UnitOfMeasure.Bag,
            Rate = 100
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
