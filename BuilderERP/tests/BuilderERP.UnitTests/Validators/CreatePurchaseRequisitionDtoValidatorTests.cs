using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePurchaseRequisitionDtoValidatorTests
{
    private readonly CreatePurchaseRequisitionDtoValidator _validator = new();

    private static List<CreatePurchaseRequisitionDetailDto> ValidDetails() => new()
    {
        new CreatePurchaseRequisitionDetailDto { MaterialId = Guid.NewGuid(), Quantity = 10, EstimatedUnitPrice = 100 }
    };

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreatePurchaseRequisitionDto { RequiredByDate = DateTime.UtcNow.AddDays(7), DepartmentId = Guid.NewGuid(), Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_have_error_when_department_id_is_empty()
    {
        var model = new CreatePurchaseRequisitionDto { RequiredByDate = DateTime.UtcNow.AddDays(7), DepartmentId = Guid.Empty, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePurchaseRequisitionDto { RequiredByDate = DateTime.UtcNow.AddDays(7), DepartmentId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
