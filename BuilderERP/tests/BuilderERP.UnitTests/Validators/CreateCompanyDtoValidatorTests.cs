using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateCompanyDtoValidatorTests
{
    private readonly CreateCompanyDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateCompanyDto { Name = string.Empty, Code = "ABC" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_email_is_invalid()
    {
        var model = new CreateCompanyDto { Name = "Acme", Code = "ABC", Email = "not-an-email" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateCompanyDto { Name = "Acme", Code = "ABC", Email = "info@acme.com" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
