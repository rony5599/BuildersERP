using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateDrawingApprovalDtoValidator : AbstractValidator<UpdateDrawingApprovalDto>
{
    public UpdateDrawingApprovalDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DrawingId).NotEmpty();
        RuleFor(x => x.ApproverName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Comments).MaximumLength(1000);
    }
}
