using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateBuildingDtoValidatorTests
{
    private readonly CreateBuildingDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_is_not_selected()
    {
        var model = new CreateBuildingDto { Name = "Tower A", Code = "TA", ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_total_floors_is_zero_or_negative()
    {
        var model = new CreateBuildingDto { Name = "Tower A", Code = "TA", ProjectId = Guid.NewGuid(), TotalFloors = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TotalFloors);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateBuildingDto { Name = "Tower A", Code = "TA", ProjectId = Guid.NewGuid(), TotalFloors = 20 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
