using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDailyProgressDtoValidatorTests
{
    private readonly CreateDailyProgressDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateDailyProgressDto { Description = "", PercentComplete = 50, ManpowerCount = 10, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_percent_complete_is_out_of_range()
    {
        var model = new CreateDailyProgressDto { Description = "Foundation work", PercentComplete = 150, ManpowerCount = 10, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PercentComplete);
    }

    [Fact]
    public void Should_have_error_when_manpower_count_is_negative()
    {
        var model = new CreateDailyProgressDto { Description = "Foundation work", PercentComplete = 50, ManpowerCount = -1, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ManpowerCount);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateDailyProgressDto { Description = "Foundation work", PercentComplete = 50, ManpowerCount = 10, ProjectId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDailyProgressDto { Description = "Foundation work", PercentComplete = 50, ManpowerCount = 10, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
