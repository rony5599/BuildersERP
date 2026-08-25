using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateNcrDtoValidatorTests
{
    private readonly CreateNcrDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_ncr_number_is_empty()
    {
        var model = new CreateNcrDto { NcrNumber = "", Description = "Cracked slab", ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.NcrNumber);
    }

    [Fact]
    public void Should_have_error_when_description_is_empty()
    {
        var model = new CreateNcrDto { NcrNumber = "NCR-001", Description = "", ProjectId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateNcrDto { NcrNumber = "NCR-001", Description = "Cracked slab", ProjectId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateNcrDto
        {
            NcrNumber = "NCR-001",
            Description = "Cracked slab",
            ProjectId = 1L,
            Severity = NcrSeverity.Major,
            Status = NcrStatus.Open
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
