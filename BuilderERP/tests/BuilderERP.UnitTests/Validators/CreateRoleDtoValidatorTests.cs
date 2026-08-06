using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateRoleDtoValidatorTests
{
    private readonly CreateRoleDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateRoleDto { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateRoleDto { Name = "Project Manager", Description = "Manages projects" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
