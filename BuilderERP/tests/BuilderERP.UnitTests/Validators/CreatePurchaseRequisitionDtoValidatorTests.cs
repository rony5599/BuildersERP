using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePurchaseRequisitionDtoValidatorTests
{
    private readonly CreatePurchaseRequisitionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_requisition_number_is_empty()
    {
        var model = new CreatePurchaseRequisitionDto { RequisitionNumber = string.Empty, RequiredByDate = DateTime.UtcNow.AddDays(7), EstimatedAmount = 1000, DepartmentId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RequisitionNumber);
    }

    [Fact]
    public void Should_have_error_when_estimated_amount_is_zero()
    {
        var model = new CreatePurchaseRequisitionDto { RequisitionNumber = "REQ-001", RequiredByDate = DateTime.UtcNow.AddDays(7), EstimatedAmount = 0, DepartmentId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstimatedAmount);
    }

    [Fact]
    public void Should_have_error_when_department_id_is_empty()
    {
        var model = new CreatePurchaseRequisitionDto { RequisitionNumber = "REQ-001", RequiredByDate = DateTime.UtcNow.AddDays(7), EstimatedAmount = 1000, DepartmentId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePurchaseRequisitionDto { RequisitionNumber = "REQ-001", RequiredByDate = DateTime.UtcNow.AddDays(7), EstimatedAmount = 1000, DepartmentId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
