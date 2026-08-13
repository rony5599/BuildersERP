using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateParkingSlotDtoValidator : AbstractValidator<UpdateParkingSlotDto>
{
    public UpdateParkingSlotDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SlotNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SlotType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AllocatedTo).MaximumLength(150);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
