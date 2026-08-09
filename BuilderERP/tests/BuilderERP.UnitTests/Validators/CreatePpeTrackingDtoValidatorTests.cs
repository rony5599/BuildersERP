using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePpeTrackingDtoValidatorTests
{
    private readonly CreatePpeTrackingDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreatePpeTrackingDto { WorkerId = Guid.Empty, PpeType = PpeType.Helmet, Status = PpeStatus.Issued };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_ppe_type_is_invalid()
    {
        var model = new CreatePpeTrackingDto { WorkerId = Guid.NewGuid(), PpeType = (PpeType)999, Status = PpeStatus.Issued };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PpeType);
    }

    [Fact]
    public void Should_have_error_when_status_is_invalid()
    {
        var model = new CreatePpeTrackingDto { WorkerId = Guid.NewGuid(), PpeType = PpeType.Helmet, Status = (PpeStatus)999 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePpeTrackingDto
        {
            WorkerId = Guid.NewGuid(),
            PpeType = PpeType.Helmet,
            IssueDate = DateTime.UtcNow,
            Status = PpeStatus.Issued
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
