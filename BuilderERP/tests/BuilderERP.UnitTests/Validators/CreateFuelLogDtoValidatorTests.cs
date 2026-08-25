using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateFuelLogDtoValidatorTests
{
    private readonly CreateFuelLogDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_equipment_id_is_empty()
    {
        var model = new CreateFuelLogDto { EquipmentId = 0L, FuelQuantity = 10, FuelCost = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentId);
    }

    [Fact]
    public void Should_have_error_when_fuel_quantity_is_negative()
    {
        var model = new CreateFuelLogDto { EquipmentId = 1L, FuelQuantity = -1, FuelCost = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FuelQuantity);
    }

    [Fact]
    public void Should_have_error_when_fuel_cost_is_negative()
    {
        var model = new CreateFuelLogDto { EquipmentId = 1L, FuelQuantity = 10, FuelCost = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FuelCost);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateFuelLogDto { EquipmentId = 1L, FuelQuantity = 10, FuelCost = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
