using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateWorkOrderDtoValidatorTests
{
    private readonly CreateWorkOrderDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_work_order_number_is_empty()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "",
            Description = "Foundation work",
            ContractorId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Amount = 1000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkOrderNumber);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "WO-001",
            Description = "",
            ContractorId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Amount = 1000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_contractor_id_is_empty()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "WO-001",
            Description = "Foundation work",
            ContractorId = Guid.Empty,
            ProjectId = Guid.NewGuid(),
            Amount = 1000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "WO-001",
            Description = "Foundation work",
            ContractorId = Guid.NewGuid(),
            ProjectId = Guid.Empty,
            Amount = 1000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_amount_is_negative()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "WO-001",
            Description = "Foundation work",
            ContractorId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Amount = -1
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateWorkOrderDto
        {
            WorkOrderNumber = "WO-001",
            Description = "Foundation work",
            ContractorId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Amount = 1000,
            Status = WorkOrderStatus.Draft
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
