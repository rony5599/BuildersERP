using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateIncidentReportDtoValidatorTests
{
    private readonly CreateIncidentReportDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateIncidentReportDto { ProjectId = 0L, ReportedBy = "John Doe", Description = "Slip and fall" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_reported_by_is_empty()
    {
        var model = new CreateIncidentReportDto { ProjectId = 1L, ReportedBy = "", Description = "Slip and fall" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReportedBy);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateIncidentReportDto { ProjectId = 1L, ReportedBy = "John Doe", Description = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateIncidentReportDto
        {
            ProjectId = 1L,
            ReportedBy = "John Doe",
            Description = "Slip and fall near the scaffolding",
            Severity = IncidentSeverity.Major,
            Status = IncidentStatus.Reported
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
