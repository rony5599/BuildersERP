namespace BuilderERP.Application.Common;

public static class LineItemCalculator
{
    public record LineAmounts(decimal DiscountAmount, decimal VatAmount, decimal TaxAmount, decimal NetAmount);

    public static LineAmounts Calculate(decimal quantity, decimal unitPrice, decimal discountPercent, decimal vatPercent, decimal taxPercent)
    {
        var gross = quantity * unitPrice;
        var discountAmount = Math.Round(gross * discountPercent / 100m, 2);
        var taxableBase = gross - discountAmount;
        var vatAmount = Math.Round(taxableBase * vatPercent / 100m, 2);
        var taxAmount = Math.Round(taxableBase * taxPercent / 100m, 2);
        var netAmount = taxableBase + vatAmount + taxAmount;
        return new LineAmounts(discountAmount, vatAmount, taxAmount, netAmount);
    }
}
