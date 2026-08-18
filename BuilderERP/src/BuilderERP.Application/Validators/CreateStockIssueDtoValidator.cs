using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class CreateStockIssueDtoValidator : AbstractValidator<CreateStockIssueDto>
{
    public CreateStockIssueDtoValidator()
    {
        RuleFor(x => x.IssueNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.IssuedTo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ConsumedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WastageQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WastageReason).MaximumLength(500);
        RuleFor(x => x)
            .Must(x => x.ConsumedQuantity + x.WastageQuantity <= x.Quantity)
            .WithMessage("Consumed quantity plus wastage quantity cannot exceed the issued quantity.")
            .WithName("ConsumedQuantity");
        RuleFor(x => x.Remarks).MaximumLength(500);
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
    }
}
