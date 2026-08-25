using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateRiskAssessmentDtoValidatorTests
{
    private readonly CreateRiskAssessmentDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateRiskAssessmentDto { ProjectId = 0L, AssessedBy = "Jane Doe", HazardDescription = "Falling debris" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_assessed_by_is_empty()
    {
        var model = new CreateRiskAssessmentDto { ProjectId = 1L, AssessedBy = "", HazardDescription = "Falling debris" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AssessedBy);
    }

    [Fact]
    public void Should_have_error_when_hazard_description_is_empty()
    {
        var model = new CreateRiskAssessmentDto { ProjectId = 1L, AssessedBy = "Jane Doe", HazardDescription = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HazardDescription);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateRiskAssessmentDto
        {
            ProjectId = 1L,
            AssessedBy = "Jane Doe",
            HazardDescription = "Falling debris near the crane zone",
            RiskLevel = RiskLevel.High
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
