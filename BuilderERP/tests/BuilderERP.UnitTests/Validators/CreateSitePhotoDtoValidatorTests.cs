using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateSitePhotoDtoValidatorTests
{
    private readonly CreateSitePhotoDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateSitePhotoDto { FilePath = "", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_have_error_when_project_id_is_empty()
    {
        var model = new CreateSitePhotoDto { FilePath = "/uploads/site-photos/abc.jpg", ProjectId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_have_error_when_caption_exceeds_max_length()
    {
        var model = new CreateSitePhotoDto { FilePath = "/uploads/site-photos/abc.jpg", Caption = new string('a', 301), ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Caption);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateSitePhotoDto { FilePath = "/uploads/site-photos/abc.jpg", Caption = "Site view", ProjectId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
