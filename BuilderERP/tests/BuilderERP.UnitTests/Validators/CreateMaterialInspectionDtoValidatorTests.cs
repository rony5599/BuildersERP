using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateMaterialInspectionDtoValidatorTests
{
    private readonly CreateMaterialInspectionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateMaterialInspectionDto { ProjectId = Guid.Empty, MaterialId = Guid.NewGuid(), InspectedBy = "John Doe", Quantity = 10 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_material_id_is_empty()
    {
        var model = new CreateMaterialInspectionDto { ProjectId = Guid.NewGuid(), MaterialId = Guid.Empty, InspectedBy = "John Doe", Quantity = 10 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.MaterialId);
    }

    [Fact]
    public void Should_have_error_when_inspected_by_is_empty()
    {
        var model = new CreateMaterialInspectionDto { ProjectId = Guid.NewGuid(), MaterialId = Guid.NewGuid(), InspectedBy = "", Quantity = 10 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.InspectedBy);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_negative()
    {
        var model = new CreateMaterialInspectionDto { ProjectId = Guid.NewGuid(), MaterialId = Guid.NewGuid(), InspectedBy = "John Doe", Quantity = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateMaterialInspectionDto
        {
            ProjectId = Guid.NewGuid(),
            MaterialId = Guid.NewGuid(),
            InspectedBy = "John Doe",
            Quantity = 10,
            Result = QcResult.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
