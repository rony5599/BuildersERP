using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using BuilderERP.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateReceiptDtoValidatorTests
{
    private readonly CreateReceiptDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_amount_paid_is_zero_or_negative()
    {
        var model = new CreateReceiptDto { ReceiptNumber = "RCT-1", AmountPaid = 0, InstallmentId = 1L, PaymentMethod = PaymentMethod.Cash };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AmountPaid);
    }

    [Fact]
    public void Should_have_error_when_receipt_number_is_empty()
    {
        var model = new CreateReceiptDto { ReceiptNumber = "", AmountPaid = 5000, InstallmentId = 1L, PaymentMethod = PaymentMethod.Cash };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReceiptNumber);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateReceiptDto { ReceiptNumber = "RCT-1", AmountPaid = 5000, InstallmentId = 1L, PaymentMethod = PaymentMethod.Cash };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
