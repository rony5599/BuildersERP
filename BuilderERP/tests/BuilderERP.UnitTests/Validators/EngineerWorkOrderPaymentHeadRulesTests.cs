using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class EngineerWorkOrderPaymentHeadRulesTests
{
    private readonly SaveEngineerWorkOrderPaymentHeadsDtoValidator _validator = new();

    private static SaveEngineerWorkOrderPaymentHeadsDto Model(params (string Name, decimal Percent)[] heads) => new()
    {
        EngineerWorkOrderId = 1L,
        PaymentHeads = heads.Select(h => new EngineerWorkOrderPaymentHeadDto { HeadName = h.Name, Percent = h.Percent }).ToList()
    };

    [Fact]
    public void Should_not_have_error_for_standard_split()
    {
        var result = _validator.TestValidate(Model(("Vertical", 18), ("Internal", 22), ("RCC", 15), ("Roof", 15), ("Finishing", 20), ("Security", 10)));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_not_have_error_when_no_heads()
    {
        var result = _validator.TestValidate(Model());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_ignore_blank_template_rows()
    {
        var result = _validator.TestValidate(Model(("Vertical", 60), ("", 0), ("Security", 40)));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_have_error_when_total_is_not_100()
    {
        var result = _validator.TestValidate(Model(("Vertical", 18), ("Internal", 22)));
        result.ShouldHaveValidationErrorFor(x => x.PaymentHeads);
    }

    [Fact]
    public void Should_have_error_when_names_repeat_ignoring_case()
    {
        var result = _validator.TestValidate(Model(("Roof", 50), ("roof ", 50)));
        result.ShouldHaveValidationErrorFor(x => x.PaymentHeads);
    }

    [Fact]
    public void Should_have_error_when_percent_has_no_name()
    {
        var result = _validator.TestValidate(Model(("Vertical", 90), ("", 10)));
        result.ShouldHaveValidationErrorFor(x => x.PaymentHeads);
    }
}
