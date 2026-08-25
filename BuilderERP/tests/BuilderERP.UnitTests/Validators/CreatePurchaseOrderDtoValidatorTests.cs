using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePurchaseOrderDtoValidatorTests
{
    private readonly CreatePurchaseOrderDtoValidator _validator = new();

    private static List<CreatePurchaseOrderDetailDto> ValidDetails() => new()
    {
        new CreatePurchaseOrderDetailDto { MaterialId = 1L, OrderedQuantity = 10, UnitPrice = 100 }
    };

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreatePurchaseOrderDto { DeliveryDate = DateTime.UtcNow.AddDays(10), VendorQuotationId = 1L, Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_have_error_when_vendor_quotation_id_is_empty()
    {
        var model = new CreatePurchaseOrderDto { DeliveryDate = DateTime.UtcNow.AddDays(10), VendorQuotationId = 0L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.VendorQuotationId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePurchaseOrderDto
        {
            DeliveryDate = DateTime.UtcNow.AddDays(10),
            VendorQuotationId = 1L,
            Status = PurchaseOrderStatus.Draft,
            Details = ValidDetails()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
