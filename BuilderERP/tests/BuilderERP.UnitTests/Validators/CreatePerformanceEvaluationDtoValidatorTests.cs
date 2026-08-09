using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePerformanceEvaluationDtoValidatorTests
{
    private readonly CreatePerformanceEvaluationDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_contractor_id_is_empty()
    {
        var model = new CreatePerformanceEvaluationDto { ContractorId = Guid.Empty, QualityScore = 3, TimelinessScore = 3, SafetyScore = 3 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
    }

    [Fact]
    public void Should_have_error_when_quality_score_is_out_of_range()
    {
        var model = new CreatePerformanceEvaluationDto { ContractorId = Guid.NewGuid(), QualityScore = 6, TimelinessScore = 3, SafetyScore = 3 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QualityScore);
    }

    [Fact]
    public void Should_have_error_when_safety_score_is_zero()
    {
        var model = new CreatePerformanceEvaluationDto { ContractorId = Guid.NewGuid(), QualityScore = 3, TimelinessScore = 3, SafetyScore = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SafetyScore);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePerformanceEvaluationDto
        {
            ContractorId = Guid.NewGuid(),
            QualityScore = 4,
            TimelinessScore = 5,
            SafetyScore = 3
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
