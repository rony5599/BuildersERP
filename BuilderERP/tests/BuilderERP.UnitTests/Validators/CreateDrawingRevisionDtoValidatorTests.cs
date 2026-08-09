using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDrawingRevisionDtoValidatorTests
{
    private readonly CreateDrawingRevisionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_revision_code_is_empty()
    {
        var model = new CreateDrawingRevisionDto { RevisionCode = "", FilePath = "/uploads/drawing-revisions/a.pdf", DrawingId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RevisionCode);
    }

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateDrawingRevisionDto { RevisionCode = "Rev A", FilePath = "", DrawingId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_have_error_when_drawing_id_is_empty()
    {
        var model = new CreateDrawingRevisionDto { RevisionCode = "Rev A", FilePath = "/uploads/drawing-revisions/a.pdf", DrawingId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DrawingId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDrawingRevisionDto
        {
            RevisionCode = "Rev A",
            FilePath = "/uploads/drawing-revisions/a.pdf",
            DrawingId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
