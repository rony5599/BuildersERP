using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateStockIssueDtoValidatorTests
{
    private readonly CreateStockIssueDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_issue_number_is_empty()
    {
        var model = new CreateStockIssueDto { IssueNumber = "", Quantity = 50, IssuedTo = "Site A", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.IssueNumber);
    }

    [Fact]
    public void Should_have_error_when_quantity_is_zero()
    {
        var model = new CreateStockIssueDto { IssueNumber = "ISS-001", Quantity = 0, IssuedTo = "Site A", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_issued_to_is_empty()
    {
        var model = new CreateStockIssueDto { IssueNumber = "ISS-001", Quantity = 50, IssuedTo = "", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.IssuedTo);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateStockIssueDto { IssueNumber = "ISS-001", Quantity = 50, IssuedTo = "Site A", MaterialId = 1L, WarehouseId = 2L };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
