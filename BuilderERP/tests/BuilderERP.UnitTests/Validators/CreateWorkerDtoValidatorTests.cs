using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateWorkerDtoValidatorTests
{
    private readonly CreateWorkerDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_code_is_empty()
    {
        var model = new CreateWorkerDto { WorkerCode = "", Name = "John Doe", Phone = "01711111111" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerCode);
    }

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var model = new CreateWorkerDto { WorkerCode = "WRK-001", Name = "", Phone = "01711111111" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_phone_is_empty()
    {
        var model = new CreateWorkerDto { WorkerCode = "WRK-001", Name = "John Doe", Phone = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Should_have_error_when_daily_wage_rate_is_negative()
    {
        var model = new CreateWorkerDto { WorkerCode = "WRK-001", Name = "John Doe", Phone = "01711111111", DailyWageRate = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DailyWageRate);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateWorkerDto
        {
            WorkerCode = "WRK-001",
            Name = "John Doe",
            Phone = "01711111111",
            Address = "123 Main St",
            Trade = "Mason",
            DailyWageRate = 800,
            JoinDate = DateTime.UtcNow
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
