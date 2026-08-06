using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateGoodsReceiveDtoValidatorTests
{
    private readonly CreateGoodsReceiveDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_grn_number_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "", ReceivedAmount = 500, PurchaseOrderId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.GrnNumber);
    }

    [Fact]
    public void Should_have_error_when_received_amount_is_zero()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", ReceivedAmount = 0, PurchaseOrderId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReceivedAmount);
    }

    [Fact]
    public void Should_have_error_when_purchase_order_id_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", ReceivedAmount = 500, PurchaseOrderId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PurchaseOrderId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", ReceivedAmount = 500, PurchaseOrderId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
