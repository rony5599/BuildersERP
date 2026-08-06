using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateWarehouseDtoValidatorTests
{
    private readonly CreateWarehouseDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_warehouse_code_is_empty()
    {
        var model = new CreateWarehouseDto { WarehouseCode = "", Name = "Main Yard", BranchId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WarehouseCode);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateWarehouseDto { WarehouseCode = "WH-01", Name = "", BranchId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_branch_id_is_empty()
    {
        var model = new CreateWarehouseDto { WarehouseCode = "WH-01", Name = "Main Yard", BranchId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BranchId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateWarehouseDto { WarehouseCode = "WH-01", Name = "Main Yard", BranchId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
