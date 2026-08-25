using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateContractorLedgerDtoValidatorTests
{
    private readonly CreateContractorLedgerDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_contractor_id_is_empty()
    {
        var model = new CreateContractorLedgerDto { ContractorId = 0L, Description = "Payment" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateContractorLedgerDto { ContractorId = 1L, Description = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_debit_amount_is_negative()
    {
        var model = new CreateContractorLedgerDto { ContractorId = 1L, Description = "Payment", DebitAmount = -100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DebitAmount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateContractorLedgerDto
        {
            ContractorId = 1L,
            Description = "Advance payment",
            DebitAmount = 5000,
            CreditAmount = 0
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
