using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateContractorDtoValidatorTests
{
    private readonly CreateContractorDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_contractor_code_is_empty()
    {
        var model = new CreateContractorDto { ContractorCode = "", Name = "Acme Contractors", Phone = "01711111111" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorCode);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateContractorDto { ContractorCode = "CON-001", Name = "", Phone = "01711111111" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_phone_is_empty()
    {
        var model = new CreateContractorDto { ContractorCode = "CON-001", Name = "Acme Contractors", Phone = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Should_have_error_when_email_is_invalid()
    {
        var model = new CreateContractorDto { ContractorCode = "CON-001", Name = "Acme Contractors", Phone = "01711111111", Email = "not-an-email" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateContractorDto
        {
            ContractorCode = "CON-001",
            Name = "Acme Contractors",
            Phone = "01711111111",
            Email = "contact@acme.com",
            ContactPerson = "John Doe",
            Address = "123 Main St",
            LicenseNumber = "LIC-123",
            Specialization = "Civil"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
