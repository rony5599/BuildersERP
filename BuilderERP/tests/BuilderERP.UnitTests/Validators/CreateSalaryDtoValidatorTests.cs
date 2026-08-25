using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSalaryDtoValidatorTests
{
    private readonly CreateSalaryDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_worker_id_is_empty()
    {
        var model = new CreateSalaryDto
        {
            WorkerId = 0L,
            PeriodStart = DateTime.UtcNow,
            PeriodEnd = DateTime.UtcNow.AddDays(30),
            DaysWorked = 22,
            BasicAmount = 10000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void Should_have_error_when_period_end_is_before_period_start()
    {
        var model = new CreateSalaryDto
        {
            WorkerId = 1L,
            PeriodStart = DateTime.UtcNow,
            PeriodEnd = DateTime.UtcNow.AddDays(-5),
            DaysWorked = 22,
            BasicAmount = 10000
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PeriodEnd);
    }

    [Fact]
    public void Should_have_error_when_basic_amount_is_negative()
    {
        var model = new CreateSalaryDto
        {
            WorkerId = 1L,
            PeriodStart = DateTime.UtcNow,
            PeriodEnd = DateTime.UtcNow.AddDays(30),
            DaysWorked = 22,
            BasicAmount = -100
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BasicAmount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSalaryDto
        {
            WorkerId = 1L,
            PeriodStart = DateTime.UtcNow,
            PeriodEnd = DateTime.UtcNow.AddDays(30),
            DaysWorked = 22.5m,
            BasicAmount = 10000,
            OvertimeAmount = 500,
            DeductionAmount = 200,
            Status = SalaryStatus.Pending
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
