using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateVisitorLogDtoValidator : AbstractValidator<UpdateVisitorLogDto>
{
    public UpdateVisitorLogDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.VisitorName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.PurposeOfVisit).MaximumLength(200);
        RuleFor(x => x.HostName).MaximumLength(150);
        RuleFor(x => x.IdProofNumber).MaximumLength(50);
        RuleFor(x => x.VehicleNumber).MaximumLength(30);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
