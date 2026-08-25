using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSafetyTrainingDtoValidatorTests
{
    private readonly CreateSafetyTrainingDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreateSafetyTrainingDto { WorkerId = 0L, TrainingTitle = "Fire Safety" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_training_title_is_empty()
    {
        var model = new CreateSafetyTrainingDto { WorkerId = 1L, TrainingTitle = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TrainingTitle);
    }

    [Fact]
    public void Should_have_error_when_duration_hours_is_negative()
    {
        var model = new CreateSafetyTrainingDto { WorkerId = 1L, TrainingTitle = "Fire Safety", DurationHours = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DurationHours);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSafetyTrainingDto
        {
            WorkerId = 1L,
            TrainingTitle = "Fire Safety",
            TrainingDate = DateTime.UtcNow,
            TrainerName = "Jane Smith",
            DurationHours = 4,
            CertificateNumber = "CERT-001",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
