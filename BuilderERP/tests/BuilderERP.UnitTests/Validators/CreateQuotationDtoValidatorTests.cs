using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateQuotationDtoValidatorTests
{
    private readonly CreateQuotationDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_quoted_price_is_zero_or_negative()
    {
        var model = new CreateQuotationDto { QuotedPrice = 0, CustomerId = 1L, PropertyUnitId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuotedPrice);
    }

    [Fact]
    public void Should_have_error_when_customer_is_not_selected()
    {
        var model = new CreateQuotationDto { QuotedPrice = 100000, CustomerId = 0L, PropertyUnitId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateQuotationDto
        {
            QuotedPrice = 250000,
            CustomerId = 1L,
            PropertyUnitId = 2L,
            Status = QuotationStatus.Draft
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
