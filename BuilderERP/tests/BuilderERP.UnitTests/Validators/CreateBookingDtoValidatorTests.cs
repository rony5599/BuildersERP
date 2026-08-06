using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateBookingDtoValidatorTests
{
    private readonly CreateBookingDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_booking_amount_is_zero_or_negative()
    {
        var model = new CreateBookingDto { BookingAmount = -1, CustomerId = Guid.NewGuid(), PropertyUnitId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BookingAmount);
    }

    [Fact]
    public void Should_have_error_when_property_unit_is_not_selected()
    {
        var model = new CreateBookingDto { BookingAmount = 5000, CustomerId = Guid.NewGuid(), PropertyUnitId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PropertyUnitId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateBookingDto
        {
            BookingAmount = 5000,
            CustomerId = Guid.NewGuid(),
            PropertyUnitId = Guid.NewGuid(),
            Status = BookingRequestStatus.Confirmed
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
