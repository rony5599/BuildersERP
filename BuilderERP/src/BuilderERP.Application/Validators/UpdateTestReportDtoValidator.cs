using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateTestReportDtoValidator : AbstractValidator<UpdateTestReportDto>
{
    public UpdateTestReportDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ReportNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TestType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
    }
}
