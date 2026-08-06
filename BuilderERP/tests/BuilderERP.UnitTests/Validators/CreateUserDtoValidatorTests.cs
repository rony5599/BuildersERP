using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateUserDtoValidatorTests
{
    private readonly CreateUserDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_email_is_invalid()
    {
        var model = new CreateUserDto { Email = "not-an-email", FullName = "Test User", Password = "Password1", Role = "Employee" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_have_error_when_password_too_short()
    {
        var model = new CreateUserDto { Email = "user@example.com", FullName = "Test User", Password = "short", Role = "Employee" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_have_error_when_role_is_empty()
    {
        var model = new CreateUserDto { Email = "user@example.com", FullName = "Test User", Password = "Password1", Role = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateUserDto { Email = "user@example.com", FullName = "Test User", Password = "Password1", Role = "Employee" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
