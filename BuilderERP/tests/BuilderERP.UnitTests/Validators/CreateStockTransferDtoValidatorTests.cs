using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateStockTransferDtoValidatorTests
{
    private readonly CreateStockTransferDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_transfer_number_is_empty()
    {
        var warehouseId = Guid.NewGuid();
        var model = new CreateStockTransferDto { TransferNumber = "", Quantity = 100, MaterialId = Guid.NewGuid(), FromWarehouseId = warehouseId, ToWarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TransferNumber);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_zero()
    {
        var model = new CreateStockTransferDto { TransferNumber = "TRF-001", Quantity = 0, MaterialId = Guid.NewGuid(), FromWarehouseId = Guid.NewGuid(), ToWarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_to_warehouse_equals_from_warehouse()
    {
        var warehouseId = Guid.NewGuid();
        var model = new CreateStockTransferDto { TransferNumber = "TRF-001", Quantity = 100, MaterialId = Guid.NewGuid(), FromWarehouseId = warehouseId, ToWarehouseId = warehouseId };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ToWarehouseId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateStockTransferDto { TransferNumber = "TRF-001", Quantity = 100, MaterialId = Guid.NewGuid(), FromWarehouseId = Guid.NewGuid(), ToWarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
