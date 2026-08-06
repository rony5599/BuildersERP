using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateMaterialDtoValidatorTests
{
    private readonly CreateMaterialDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_material_code_is_empty()
    {
        var model = new CreateMaterialDto { MaterialCode = "", Name = "Cement", UnitOfMeasure = UnitOfMeasure.Bag, ReorderLevel = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.MaterialCode);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateMaterialDto { MaterialCode = "MAT-01", Name = "", UnitOfMeasure = UnitOfMeasure.Bag, ReorderLevel = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_reorder_level_is_negative()
    {
        var model = new CreateMaterialDto { MaterialCode = "MAT-01", Name = "Cement", UnitOfMeasure = UnitOfMeasure.Bag, ReorderLevel = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReorderLevel);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateMaterialDto { MaterialCode = "MAT-01", Name = "Cement", UnitOfMeasure = UnitOfMeasure.Bag, ReorderLevel = 50 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
