using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateLandMutationDtoValidator : AbstractValidator<UpdateLandMutationDto>
{
    public UpdateLandMutationDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.MutationNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ApplicantName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.KhatianNumber).MaximumLength(50);
        RuleFor(x => x.DagNumber).MaximumLength(50);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
