using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSafetyInspectionDtoValidatorTests
{
    private readonly CreateSafetyInspectionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateSafetyInspectionDto { ProjectId = Guid.Empty, InspectedBy = "John Doe" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_inspected_by_is_empty()
    {
        var model = new CreateSafetyInspectionDto { ProjectId = Guid.NewGuid(), InspectedBy = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.InspectedBy);
    }

    [Fact]
    public void Should_have_error_when_result_is_invalid()
    {
        var model = new CreateSafetyInspectionDto { ProjectId = Guid.NewGuid(), InspectedBy = "John Doe", Result = (QcResult)999 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Result);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSafetyInspectionDto
        {
            ProjectId = Guid.NewGuid(),
            InspectedBy = "John Doe",
            Location = "Site A",
            Result = QcResult.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
