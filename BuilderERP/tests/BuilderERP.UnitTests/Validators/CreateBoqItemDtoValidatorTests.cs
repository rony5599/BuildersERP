using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateBoqItemDtoValidatorTests
{
    private readonly CreateBoqItemDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_item_code_is_empty()
    {
        var model = new CreateBoqItemDto { ItemCode = "", Description = "Excavation work", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = 10, Rate = 5, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ItemCode);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateBoqItemDto { ItemCode = "BOQ-01", Description = "", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = 10, Rate = 5, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_negative()
    {
        var model = new CreateBoqItemDto { ItemCode = "BOQ-01", Description = "Excavation work", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = -1, Rate = 5, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_rate_is_negative()
    {
        var model = new CreateBoqItemDto { ItemCode = "BOQ-01", Description = "Excavation work", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = 10, Rate = -5, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Rate);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateBoqItemDto { ItemCode = "BOQ-01", Description = "Excavation work", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = 10, Rate = 5, ProjectId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateBoqItemDto { ItemCode = "BOQ-01", Description = "Excavation work", UnitOfMeasure = UnitOfMeasure.CubicFeet, Quantity = 10, Rate = 5, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
