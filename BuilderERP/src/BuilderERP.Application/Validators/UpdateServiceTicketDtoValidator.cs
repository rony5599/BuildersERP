using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateServiceTicketDtoValidator : AbstractValidator<UpdateServiceTicketDto>
{
    public UpdateServiceTicketDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PropertyUnitId).NotEmpty();
        RuleFor(x => x.TicketNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AssignedTo).MaximumLength(150);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
