using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateTestReportDtoValidator : AbstractValidator<CreateTestReportDto>
{
    public CreateTestReportDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ReportNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TestType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Result).IsInEnum();
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(500);
    }
}
