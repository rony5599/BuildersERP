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
        new CreateGoodsReceiveDetailDto { PurchaseOrderDetailId = Guid.NewGuid(), MaterialId = Guid.NewGuid(), ReceivedQuantity = 10, UnitPrice = 50 }
    };

    [Fact]
    public void Should_have_error_when_grn_number_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "", PurchaseOrderId = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.GrnNumber);
    }

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", PurchaseOrderId = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_have_error_when_purchase_order_id_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", PurchaseOrderId = Guid.Empty, WarehouseId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PurchaseOrderId);
    }

    [Fact]
    public void Should_have_error_when_warehouse_id_is_empty()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", PurchaseOrderId = Guid.NewGuid(), WarehouseId = Guid.Empty, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WarehouseId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateGoodsReceiveDto { GrnNumber = "GRN-001", PurchaseOrderId = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
