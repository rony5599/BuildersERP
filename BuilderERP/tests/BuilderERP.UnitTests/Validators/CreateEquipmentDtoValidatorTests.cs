using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateEquipmentDtoValidatorTests
{
    private readonly CreateEquipmentDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_equipment_code_is_empty()
    {
        var model = new CreateEquipmentDto { EquipmentCode = "", Name = "Excavator 1", Type = EquipmentType.Excavator, Status = EquipmentStatus.Available };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentCode);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateEquipmentDto { EquipmentCode = "EQ-01", Name = "", Type = EquipmentType.Excavator, Status = EquipmentStatus.Available };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_type_is_invalid()
    {
        var model = new CreateEquipmentDto { EquipmentCode = "EQ-01", Name = "Excavator 1", Type = (EquipmentType)999, Status = EquipmentStatus.Available };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateEquipmentDto { EquipmentCode = "EQ-01", Name = "Excavator 1", Type = EquipmentType.Excavator, Status = EquipmentStatus.Available };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
