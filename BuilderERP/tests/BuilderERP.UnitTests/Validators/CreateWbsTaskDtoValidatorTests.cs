using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateWbsTaskDtoValidatorTests
{
    private readonly CreateWbsTaskDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_code_is_empty()
    {
        var model = new CreateWbsTaskDto { Code = "", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_end_date_is_before_start_date()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(-1), ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_have_error_when_percent_complete_is_out_of_range()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), PercentComplete = 150, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PercentComplete);
    }

    [Fact]
    public void Should_have_error_when_sequence_is_negative()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), Sequence = -1, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Sequence);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), ProjectId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateWbsTaskDto { Code = "WBS-01", Name = "Foundation", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10), PercentComplete = 25, Sequence = 1, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
