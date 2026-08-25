using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePropertyUnitDtoValidatorTests
{
    private readonly CreatePropertyUnitDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_unit_number_is_empty()
    {
        var model = new CreatePropertyUnitDto { UnitNumber = "", FloorId = 1L, UnitType = UnitType.Flat };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UnitNumber);
    }

    [Fact]
    public void Should_have_error_when_price_is_negative()
    {
        var model = new CreatePropertyUnitDto { UnitNumber = "A-101", FloorId = 1L, UnitType = UnitType.Flat, Price = -100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_have_error_when_area_is_negative()
    {
        var model = new CreatePropertyUnitDto { UnitNumber = "A-101", FloorId = 1L, UnitType = UnitType.Flat, Area = -50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Area);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePropertyUnitDto
        {
            UnitNumber = "A-101",
            FloorId = 1L,
            UnitType = UnitType.Flat,
            Area = 1200,
            Price = 250000,
            BookingStatus = BookingStatus.Available
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
