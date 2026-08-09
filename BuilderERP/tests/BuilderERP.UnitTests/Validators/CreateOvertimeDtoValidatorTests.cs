using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateOvertimeDtoValidatorTests
{
    private readonly CreateOvertimeDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreateOvertimeDto { WorkerId = Guid.Empty, Hours = 2, RatePerHour = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_hours_is_negative()
    {
        var model = new CreateOvertimeDto { WorkerId = Guid.NewGuid(), Hours = -1, RatePerHour = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Hours);
    }

    [Fact]
    public void Should_have_error_when_rate_per_hour_is_negative()
    {
        var model = new CreateOvertimeDto { WorkerId = Guid.NewGuid(), Hours = 2, RatePerHour = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RatePerHour);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateOvertimeDto
        {
            WorkerId = Guid.NewGuid(),
            Hours = 3,
            RatePerHour = 150,
            OvertimeDate = DateTime.UtcNow
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
