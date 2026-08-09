using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateDocumentDtoValidatorTests
{
    private readonly CreateDocumentDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_document_number_is_empty()
    {
        var model = new CreateDocumentDto { DocumentNumber = "", Title = "Sale Deed", FilePath = "/uploads/documents/abc.pdf" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
    }

    [Fact]
    public void Should_have_error_when_title_is_empty()
    {
        var model = new CreateDocumentDto { DocumentNumber = "DOC-001", Title = "", FilePath = "/uploads/documents/abc.pdf" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_have_error_when_file_path_is_empty()
    {
        var model = new CreateDocumentDto { DocumentNumber = "DOC-001", Title = "Sale Deed", FilePath = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FilePath);
    }

    [Fact]
    public void Should_have_error_when_document_type_is_invalid()
    {
        var model = new CreateDocumentDto
        {
            DocumentNumber = "DOC-001",
            Title = "Sale Deed",
            FilePath = "/uploads/documents/abc.pdf",
            DocumentType = (DocumentType)999
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateDocumentDto
        {
            DocumentNumber = "DOC-001",
            Title = "Sale Deed",
            DocumentType = DocumentType.SaleDeed,
            FilePath = "/uploads/documents/abc.pdf"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
