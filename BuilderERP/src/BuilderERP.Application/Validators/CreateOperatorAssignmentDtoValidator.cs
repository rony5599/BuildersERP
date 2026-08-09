using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateOperatorAssignmentDtoValidator : AbstractValidator<CreateOperatorAssignmentDto>
{
    public CreateOperatorAssignmentDtoValidator()
    {
        RuleFor(x => x.EquipmentId).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
