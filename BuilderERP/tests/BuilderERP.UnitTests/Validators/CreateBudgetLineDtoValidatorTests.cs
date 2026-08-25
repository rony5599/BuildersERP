using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateBudgetLineDtoValidatorTests
{
    private readonly CreateBudgetLineDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateBudgetLineDto { ProjectId = 0L, Category = "Material", BudgetedAmount = 1000, ActualAmount = 500 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_category_is_empty()
    {
        var model = new CreateBudgetLineDto { ProjectId = 1L, Category = "", BudgetedAmount = 1000, ActualAmount = 500 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Should_have_error_when_budgeted_amount_is_negative()
    {
        var model = new CreateBudgetLineDto { ProjectId = 1L, Category = "Material", BudgetedAmount = -1, ActualAmount = 500 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BudgetedAmount);
    }

    [Fact]
    public void Should_have_error_when_actual_amount_is_negative()
    {
        var model = new CreateBudgetLineDto { ProjectId = 1L, Category = "Material", BudgetedAmount = 1000, ActualAmount = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ActualAmount);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateBudgetLineDto { ProjectId = 1L, Category = "Material", BudgetedAmount = 1000, ActualAmount = 500 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
