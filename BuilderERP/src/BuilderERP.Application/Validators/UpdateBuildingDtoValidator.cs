using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateBuildingDtoValidator : AbstractValidator<UpdateBuildingDto>
{
    public UpdateBuildingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TotalFloors).GreaterThan(0).When(x => x.TotalFloors.HasValue);
    }
}
