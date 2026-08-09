using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateAttendanceDtoValidator : AbstractValidator<UpdateAttendanceDto>
{
    public UpdateAttendanceDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.HoursWorked).GreaterThanOrEqualTo(0);
    }
}
