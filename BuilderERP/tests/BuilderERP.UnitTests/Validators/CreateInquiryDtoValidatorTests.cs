using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateInquiryDtoValidatorTests
{
    private readonly CreateInquiryDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_message_is_empty()
    {
        var model = new CreateInquiryDto { Message = "", LeadId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_have_error_when_lead_is_not_selected()
    {
        var model = new CreateInquiryDto { Message = "Interested in a unit", LeadId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LeadId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model_without_property_unit()
    {
        var model = new CreateInquiryDto { Message = "Interested in a unit", LeadId = Guid.NewGuid(), PropertyUnitId = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
