using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateOperatorAssignmentDtoValidatorTests
{
    private readonly CreateOperatorAssignmentDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_equipment_id_is_empty()
    {
        var model = new CreateOperatorAssignmentDto { EquipmentId = Guid.Empty, WorkerId = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentId);
    }

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreateOperatorAssignmentDto { EquipmentId = Guid.NewGuid(), WorkerId = Guid.Empty, ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateOperatorAssignmentDto { EquipmentId = Guid.NewGuid(), WorkerId = Guid.NewGuid(), ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateOperatorAssignmentDto
        {
            EquipmentId = Guid.NewGuid(),
            WorkerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
