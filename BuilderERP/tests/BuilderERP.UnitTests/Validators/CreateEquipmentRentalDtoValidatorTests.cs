using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateEquipmentRentalDtoValidatorTests
{
    private readonly CreateEquipmentRentalDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_equipment_id_is_empty()
    {
        var model = new CreateEquipmentRentalDto { EquipmentId = Guid.Empty, ProjectId = Guid.NewGuid(), RatePerDay = 100, Status = RentalStatus.Active };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentId);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateEquipmentRentalDto { EquipmentId = Guid.NewGuid(), ProjectId = Guid.Empty, RatePerDay = 100, Status = RentalStatus.Active };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_rate_per_day_is_negative()
    {
        var model = new CreateEquipmentRentalDto { EquipmentId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), RatePerDay = -1, Status = RentalStatus.Active };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RatePerDay);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateEquipmentRentalDto { EquipmentId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), RatePerDay = 100, Status = RentalStatus.Active };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
