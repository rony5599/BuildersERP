using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSiteInspectionDtoValidatorTests
{
    private readonly CreateSiteInspectionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateSiteInspectionDto { ProjectId = Guid.Empty, InspectedBy = "John Doe" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_inspected_by_is_empty()
    {
        var model = new CreateSiteInspectionDto { ProjectId = Guid.NewGuid(), InspectedBy = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.InspectedBy);
    }

    [Fact]
    public void Should_have_error_when_location_exceeds_max_length()
    {
        var model = new CreateSiteInspectionDto { ProjectId = Guid.NewGuid(), InspectedBy = "John Doe", Location = new string('A', 201) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSiteInspectionDto
        {
            ProjectId = Guid.NewGuid(),
            InspectedBy = "John Doe",
            Location = "Block A",
            Result = QcResult.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
