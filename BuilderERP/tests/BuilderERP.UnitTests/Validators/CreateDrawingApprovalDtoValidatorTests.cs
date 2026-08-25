using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDrawingApprovalDtoValidatorTests
{
    private readonly CreateDrawingApprovalDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_drawing_id_is_empty()
    {
        var model = new CreateDrawingApprovalDto { DrawingId = 0L, ApproverName = "John Doe", Status = ApprovalStatus.Pending };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DrawingId);
    }

    [Fact]
    public void Should_have_error_when_approver_name_is_empty()
    {
        var model = new CreateDrawingApprovalDto { DrawingId = 1L, ApproverName = "", Status = ApprovalStatus.Pending };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ApproverName);
    }

    [Fact]
    public void Should_have_error_when_approver_name_is_too_long()
    {
        var model = new CreateDrawingApprovalDto { DrawingId = 1L, ApproverName = new string('A', 201), Status = ApprovalStatus.Pending };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ApproverName);
    }

    [Fact]
    public void Should_have_error_when_status_is_invalid_enum()
    {
        var model = new CreateDrawingApprovalDto { DrawingId = 1L, ApproverName = "John Doe", Status = (ApprovalStatus)999 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDrawingApprovalDto { DrawingId = 1L, ApproverName = "John Doe", Status = ApprovalStatus.Pending };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
