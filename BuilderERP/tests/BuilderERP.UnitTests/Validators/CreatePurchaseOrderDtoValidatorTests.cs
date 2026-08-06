using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePurchaseOrderDtoValidatorTests
{
    private readonly CreatePurchaseOrderDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_po_number_is_empty()
    {
        var model = new CreatePurchaseOrderDto { PONumber = "", TotalAmount = 1000, DeliveryDate = DateTime.UtcNow.AddDays(10), VendorQuotationId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PONumber);
    }

    [Fact]
    public void Should_have_error_when_total_amount_is_zero()
    {
        var model = new CreatePurchaseOrderDto { PONumber = "PO-001", TotalAmount = 0, DeliveryDate = DateTime.UtcNow.AddDays(10), VendorQuotationId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
    }

    [Fact]
    public void Should_have_error_when_vendor_quotation_id_is_empty()
    {
        var model = new CreatePurchaseOrderDto { PONumber = "PO-001", TotalAmount = 1000, DeliveryDate = DateTime.UtcNow.AddDays(10), VendorQuotationId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.VendorQuotationId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePurchaseOrderDto
        {
            PONumber = "PO-001",
            TotalAmount = 1000,
            DeliveryDate = DateTime.UtcNow.AddDays(10),
            VendorQuotationId = Guid.NewGuid(),
            Status = PurchaseOrderStatus.Draft
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
