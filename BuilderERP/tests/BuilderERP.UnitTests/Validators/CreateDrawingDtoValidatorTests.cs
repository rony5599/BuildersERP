using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDrawingDtoValidatorTests
{
    private readonly CreateDrawingDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_drawing_number_is_empty()
    {
        var model = new CreateDrawingDto { DrawingNumber = "", Title = "Ground Floor Plan", FilePath = "/uploads/drawings/a.pdf", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DrawingNumber);
    }

    [Fact]
    public void Should_have_error_when_title_is_empty()
    {
        var model = new CreateDrawingDto { DrawingNumber = "DWG-001", Title = "", FilePath = "/uploads/drawings/a.pdf", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateDrawingDto { DrawingNumber = "DWG-001", Title = "Ground Floor Plan", FilePath = "", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateDrawingDto { DrawingNumber = "DWG-001", Title = "Ground Floor Plan", FilePath = "/uploads/drawings/a.pdf", ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDrawingDto
        {
            DrawingNumber = "DWG-001",
            Title = "Ground Floor Plan",
            Discipline = DrawingDiscipline.Architectural,
            FilePath = "/uploads/drawings/a.pdf",
            Status = DrawingStatus.Draft,
            ProjectId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
