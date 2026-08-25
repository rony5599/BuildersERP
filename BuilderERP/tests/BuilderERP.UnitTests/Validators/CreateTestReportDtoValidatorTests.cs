using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateTestReportDtoValidatorTests
{
    private readonly CreateTestReportDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_report_number_is_empty()
    {
        var model = new CreateTestReportDto { ReportNumber = "", TestType = "Concrete Cube Test", ProjectId = 1L, FilePath = "/uploads/test-reports/file.pdf" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReportNumber);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateTestReportDto { ReportNumber = "TR-001", TestType = "Concrete Cube Test", ProjectId = 0L, FilePath = "/uploads/test-reports/file.pdf" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateTestReportDto { ReportNumber = "TR-001", TestType = "Concrete Cube Test", ProjectId = 1L, FilePath = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateTestReportDto
        {
            ReportNumber = "TR-001",
            TestType = "Concrete Cube Test",
            ProjectId = 1L,
            FilePath = "/uploads/test-reports/file.pdf",
            Result = QcResult.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
