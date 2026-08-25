using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateQualityChecklistDtoValidatorTests
{
    private readonly CreateQualityChecklistDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateQualityChecklistDto { ProjectId = 0L, ChecklistName = "Concrete Pour", CheckedBy = "John Doe" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_checklist_name_is_empty()
    {
        var model = new CreateQualityChecklistDto { ProjectId = 1L, ChecklistName = "", CheckedBy = "John Doe" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ChecklistName);
    }

    [Fact]
    public void Should_have_error_when_checked_by_is_empty()
    {
        var model = new CreateQualityChecklistDto { ProjectId = 1L, ChecklistName = "Concrete Pour", CheckedBy = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CheckedBy);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateQualityChecklistDto
        {
            ProjectId = 1L,
            ChecklistName = "Concrete Pour",
            Category = "Concrete Pour",
            CheckedBy = "John Doe",
            Result = QcResult.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
