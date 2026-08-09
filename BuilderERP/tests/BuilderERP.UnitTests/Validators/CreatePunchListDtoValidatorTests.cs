using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePunchListDtoValidatorTests
{
    private readonly CreatePunchListDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_item_number_is_empty()
    {
        var model = new CreatePunchListDto { ItemNumber = "", Description = "Paint touch-up", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ItemNumber);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreatePunchListDto { ItemNumber = "PL-001", Description = "", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreatePunchListDto { ItemNumber = "PL-001", Description = "Paint touch-up", ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePunchListDto
        {
            ItemNumber = "PL-001",
            Description = "Paint touch-up",
            ProjectId = Guid.NewGuid(),
            Status = PunchListStatus.Open
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
