using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateOperatorAssignmentDtoValidator : AbstractValidator<UpdateOperatorAssignmentDto>
{
    public UpdateOperatorAssignmentDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
