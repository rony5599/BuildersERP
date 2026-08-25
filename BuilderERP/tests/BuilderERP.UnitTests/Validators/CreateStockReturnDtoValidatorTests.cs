using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateStockReturnDtoValidatorTests
{
    private readonly CreateStockReturnDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_return_number_is_empty()
    {
        var model = new CreateStockReturnDto { ReturnNumber = "", Quantity = 30, Reason = "Unused", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReturnNumber);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_zero()
    {
        var model = new CreateStockReturnDto { ReturnNumber = "RET-001", Quantity = 0, Reason = "Unused", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_reason_is_empty()
    {
        var model = new CreateStockReturnDto { ReturnNumber = "RET-001", Quantity = 30, Reason = "", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateStockReturnDto { ReturnNumber = "RET-001", Quantity = 30, Reason = "Unused", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
