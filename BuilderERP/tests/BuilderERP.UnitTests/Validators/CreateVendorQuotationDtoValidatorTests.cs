using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateVendorQuotationDtoValidatorTests
{
    private readonly CreateVendorQuotationDtoValidator _validator = new();

    private static List<CreateVendorQuotationDetailDto> ValidDetails() => new()
    {
        new CreateVendorQuotationDetailDto { MaterialId = 1L, Quantity = 10, UnitPrice = 100 }
    };

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreateVendorQuotationDto { DeliveryDays = 5, RfqId = 1L, SupplierId = 2L, Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_have_error_when_rfq_id_is_empty()
    {
        var model = new CreateVendorQuotationDto { DeliveryDays = 5, RfqId = 0L, SupplierId = 1L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RfqId);
    }

    [Fact]
    public void Should_have_error_when_supplier_id_is_empty()
    {
        var model = new CreateVendorQuotationDto { DeliveryDays = 5, RfqId = 1L, SupplierId = 0L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SupplierId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateVendorQuotationDto
        {
            DeliveryDays = 5,
            RfqId = 1L,
            SupplierId = 2L,
            Status = VendorQuotationStatus.Received,
            Details = ValidDetails()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
