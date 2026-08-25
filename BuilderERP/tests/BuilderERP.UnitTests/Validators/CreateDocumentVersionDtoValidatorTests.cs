using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDocumentVersionDtoValidatorTests
{
    private readonly CreateDocumentVersionDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_version_number_is_empty()
    {
        var model = new CreateDocumentVersionDto { VersionNumber = "", FilePath = "/uploads/document-versions/abc.pdf", DocumentId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.VersionNumber);
    }

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateDocumentVersionDto { VersionNumber = "V1", FilePath = "", DocumentId = 1L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_have_error_when_document_id_is_empty()
    {
        var model = new CreateDocumentVersionDto { VersionNumber = "V1", FilePath = "/uploads/document-versions/abc.pdf", DocumentId = 0L };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDocumentVersionDto
        {
            VersionNumber = "V1",
            FilePath = "/uploads/document-versions/abc.pdf",
            DocumentId = 1L
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
