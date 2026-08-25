using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateMaintenanceRecordDtoValidatorTests
{
    private readonly CreateMaintenanceRecordDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_equipment_id_is_empty()
    {
        var model = new CreateMaintenanceRecordDto { EquipmentId = 0L, Description = "Oil change", Cost = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentId);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateMaintenanceRecordDto { EquipmentId = 1L, Description = "", Cost = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_cost_is_negative()
    {
        var model = new CreateMaintenanceRecordDto { EquipmentId = 1L, Description = "Oil change", Cost = -50 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Cost);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateMaintenanceRecordDto
        {
            EquipmentId = 1L,
            Description = "Routine oil change and inspection",
            Cost = 100,
            MaintenanceType = MaintenanceType.Routine,
            Status = MaintenanceStatus.Scheduled
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
