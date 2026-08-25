using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateMilestoneDtoValidatorTests
{
    private readonly CreateMilestoneDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateMilestoneDto { Name = "", TargetDate = DateTime.Today, Status = MilestoneStatus.Pending, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_status_is_invalid()
    {
        var model = new CreateMilestoneDto { Name = "Slab Casting", TargetDate = DateTime.Today, Status = (MilestoneStatus)999, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateMilestoneDto { Name = "Slab Casting", TargetDate = DateTime.Today, Status = MilestoneStatus.Pending, ProjectId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateMilestoneDto { Name = "Slab Casting", TargetDate = DateTime.Today, Status = MilestoneStatus.Pending, ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
