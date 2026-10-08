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
    public void Should_have_error_when_work_group_is_empty()
    {
        var model = ValidModel();
        model.WorkGroupId = 0;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkGroupId);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = ValidModel();
        model.Description = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_negative()
    {
        var model = ValidModel();
        model.Quantity = -1;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_rate_is_negative()
    {
        var model = ValidModel();
        model.Rate = -5;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Rate);
    }

    [Fact]
    public void Should_have_error_when_boq_id_is_empty()
    {
        var model = ValidModel();
        model.BoqId = 0;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BoqId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = ValidModel();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateBoqItemDto ValidModel() => new()
    {
        BoqId = 1,
        WorkGroupId = 1,
        Description = "Excavation work",
        UnitOfMeasure = UnitOfMeasure.CubicFeet,
        Quantity = 10,
        Rate = 5
    };
}
