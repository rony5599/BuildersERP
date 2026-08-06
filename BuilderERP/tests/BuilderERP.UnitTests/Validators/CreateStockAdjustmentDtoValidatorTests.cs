using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateStockAdjustmentDtoValidatorTests
{
    private readonly CreateStockAdjustmentDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_adjustment_number_is_empty()
    {
        var model = new CreateStockAdjustmentDto { AdjustmentNumber = "", QuantityDelta = 500, Reason = "Opening stock", MaterialId = Guid.NewGuid(), WarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AdjustmentNumber);
    }

    [Fact]
    public void Should_have_error_when_quantity_delta_is_zero()
    {
        var model = new CreateStockAdjustmentDto { AdjustmentNumber = "ADJ-001", QuantityDelta = 0, Reason = "Opening stock", MaterialId = Guid.NewGuid(), WarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuantityDelta);
    }

    [Fact]
    public void Should_not_have_error_when_quantity_delta_is_negative()
    {
        var model = new CreateStockAdjustmentDto { AdjustmentNumber = "ADJ-001", QuantityDelta = -25, Reason = "Damaged", MaterialId = Guid.NewGuid(), WarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.QuantityDelta);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateStockAdjustmentDto { AdjustmentNumber = "ADJ-001", QuantityDelta = 500, Reason = "Opening stock", MaterialId = Guid.NewGuid(), WarehouseId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
