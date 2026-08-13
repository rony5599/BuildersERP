using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateLandDocumentDtoValidator : AbstractValidator<UpdateLandDocumentDto>
{
    public UpdateLandDocumentDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LandDocumentType).IsInEnum();
        RuleFor(x => x.MouzaName).MaximumLength(150);
        RuleFor(x => x.JlNumber).MaximumLength(50);
        RuleFor(x => x.KhatianNumber).MaximumLength(50);
        RuleFor(x => x.DagNumber).MaximumLength(50);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
