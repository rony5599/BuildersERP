using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSafetyAuditDtoValidatorTests
{
    private readonly CreateSafetyAuditDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateSafetyAuditDto { ProjectId = 0L, AuditedBy = "Jane Doe", Score = 80 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_audited_by_is_empty()
    {
        var model = new CreateSafetyAuditDto { ProjectId = 1L, AuditedBy = "", Score = 80 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuditedBy);
    }

    [Fact]
    public void Should_have_error_when_score_is_out_of_range()
    {
        var model = new CreateSafetyAuditDto { ProjectId = 1L, AuditedBy = "Jane Doe", Score = 150 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Score);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSafetyAuditDto
        {
            ProjectId = 1L,
            AuditedBy = "Jane Doe",
            Score = 85,
            Status = SafetyAuditStatus.Scheduled
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
