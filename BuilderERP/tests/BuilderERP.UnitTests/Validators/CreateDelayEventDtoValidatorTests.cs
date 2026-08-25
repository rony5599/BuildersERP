using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDelayEventDtoValidatorTests
{
    private readonly CreateDelayEventDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateDelayEventDto { ProjectId = 0L, Description = "Heavy rain", DelayDays = 3, Reason = DelayReason.Weather };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateDelayEventDto { ProjectId = 1L, Description = "", DelayDays = 3, Reason = DelayReason.Weather };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_delay_days_is_negative()
    {
        var model = new CreateDelayEventDto { ProjectId = 1L, Description = "Heavy rain", DelayDays = -1, Reason = DelayReason.Weather };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DelayDays);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDelayEventDto { ProjectId = 1L, Description = "Heavy rain", DelayDays = 3, Reason = DelayReason.Weather };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
