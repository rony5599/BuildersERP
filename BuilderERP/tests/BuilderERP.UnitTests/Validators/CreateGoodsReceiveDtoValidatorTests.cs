using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateGoodsReceiveDtoValidatorTests
{
    private readonly CreateGoodsReceiveDtoValidator _validator = new();

    private static List<CreateGoodsReceiveDetailDto> ValidDetails() => new()
    {
        new CreateGoodsReceiveDetailDto { PurchaseOrderDetailId = 1L, MaterialId = 2L, ReceivedQuantity = 10, UnitPrice = 50 }
    };

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreateGoodsReceiveDto { PurchaseOrderId = 1L, WarehouseId = 2L, Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_have_error_when_purchase_order_id_is_empty()
    {
        var model = new CreateGoodsReceiveDto { PurchaseOrderId = 0L, WarehouseId = 1L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PurchaseOrderId);
    }

    [Fact]
    public void Should_have_error_when_warehouse_id_is_empty()
    {
        var model = new CreateGoodsReceiveDto { PurchaseOrderId = 1L, WarehouseId = 0L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WarehouseId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateGoodsReceiveDto { PurchaseOrderId = 1L, WarehouseId = 2L, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
