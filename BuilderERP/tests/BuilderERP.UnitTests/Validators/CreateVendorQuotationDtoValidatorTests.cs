using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateVendorQuotationDtoValidatorTests
{
    private readonly CreateVendorQuotationDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_quotation_number_is_empty()
    {
        var model = new CreateVendorQuotationDto { QuotationNumber = "", QuotedAmount = 1000, DeliveryDays = 5, RfqId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuotationNumber);
    }

    [Fact]
    public void Should_have_error_when_quoted_amount_is_zero()
    {
        var model = new CreateVendorQuotationDto { QuotationNumber = "VQ-001", QuotedAmount = 0, DeliveryDays = 5, RfqId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuotedAmount);
    }

    [Fact]
    public void Should_have_error_when_rfq_id_is_empty()
    {
        var model = new CreateVendorQuotationDto { QuotationNumber = "VQ-001", QuotedAmount = 1000, DeliveryDays = 5, RfqId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RfqId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateVendorQuotationDto
        {
            QuotationNumber = "VQ-001",
            QuotedAmount = 1000,
            DeliveryDays = 5,
            RfqId = Guid.NewGuid(),
            Status = VendorQuotationStatus.Received
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
