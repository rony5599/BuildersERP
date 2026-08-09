using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateRunningBillDtoValidatorTests
{
    private readonly CreateRunningBillDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_bill_number_is_empty()
    {
        var model = new CreateRunningBillDto { BillNumber = "", WorkOrderId = Guid.NewGuid(), WorkDoneAmount = 1000 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BillNumber);
    }

    [Fact]
    public void Should_have_error_when_work_order_id_is_empty()
    {
        var model = new CreateRunningBillDto { BillNumber = "RB-001", WorkOrderId = Guid.Empty, WorkDoneAmount = 1000 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkOrderId);
    }

    [Fact]
    public void Should_have_error_when_work_done_amount_is_negative()
    {
        var model = new CreateRunningBillDto { BillNumber = "RB-001", WorkOrderId = Guid.NewGuid(), WorkDoneAmount = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkDoneAmount);
    }

    [Fact]
    public void Should_have_error_when_deduction_amount_is_negative()
    {
        var model = new CreateRunningBillDto { BillNumber = "RB-001", WorkOrderId = Guid.NewGuid(), WorkDoneAmount = 1000, DeductionAmount = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DeductionAmount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateRunningBillDto
        {
            BillNumber = "RB-001",
            WorkOrderId = Guid.NewGuid(),
            WorkDoneAmount = 1000,
            PreviousBillAmount = 200,
            DeductionAmount = 50,
            Status = RunningBillStatus.Draft
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
