using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateLeadDtoValidatorTests
{
    private readonly CreateLeadDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_phone_is_empty()
    {
        var model = new CreateLeadDto { Name = "Jane Doe", Phone = "", Status = LeadStatus.New };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Should_have_error_when_email_is_invalid()
    {
        var model = new CreateLeadDto { Name = "Jane Doe", Phone = "0123456789", Email = "not-an-email", Status = LeadStatus.New };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateLeadDto { Name = "Jane Doe", Phone = "0123456789", Email = "jane@example.com", Status = LeadStatus.New };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
