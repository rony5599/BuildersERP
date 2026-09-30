using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrders;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public static class EngineerWorkOrderPaymentHeadRules
{
    // Heads are optional on a work order, but when given they must add up to exactly 100%.
    public static IRuleBuilderOptions<T, List<EngineerWorkOrderPaymentHeadDto>> ValidPaymentHeads<T>(
        this IRuleBuilder<T, List<EngineerWorkOrderPaymentHeadDto>> ruleBuilder)
    {
        return ruleBuilder
            .Must(heads => EngineerWorkOrderPaymentHeads.Clean(heads).All(h => h.HeadName.Length is > 0 and <= 100))
            .WithMessage("Every payment head needs a name of up to 100 characters.")
            .Must(heads => EngineerWorkOrderPaymentHeads.Clean(heads).All(h => h.Percent is > 0 and <= 100 && decimal.Round(h.Percent, 2) == h.Percent))
            .WithMessage("Each payment head percent must be greater than 0, at most 100, with up to 2 decimals.")
            .Must(heads =>
            {
                var cleaned = EngineerWorkOrderPaymentHeads.Clean(heads);
                return cleaned.Select(h => EngineerWorkOrderPaymentHeads.Key(h.HeadName)).Distinct().Count() == cleaned.Count;
            })
            .WithMessage("Payment head names must be unique.")
            .Must(heads =>
            {
                var cleaned = EngineerWorkOrderPaymentHeads.Clean(heads);
                return cleaned.Count == 0 || cleaned.Sum(h => h.Percent) == 100m;
            })
            .WithMessage("Payment heads must total exactly 100%.");
    }
}
