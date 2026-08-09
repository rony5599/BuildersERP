using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateAttendanceDtoValidatorTests
{
    private readonly CreateAttendanceDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreateAttendanceDto { WorkerId = Guid.Empty, ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateAttendanceDto { WorkerId = Guid.NewGuid(), ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_status_is_invalid()
    {
        var model = new CreateAttendanceDto { WorkerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Status = (AttendanceStatus)999 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_have_error_when_hours_worked_is_negative()
    {
        var model = new CreateAttendanceDto { WorkerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), HoursWorked = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HoursWorked);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateAttendanceDto
        {
            WorkerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            AttendanceDate = DateTime.UtcNow,
            Status = AttendanceStatus.Present,
            HoursWorked = 8
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
