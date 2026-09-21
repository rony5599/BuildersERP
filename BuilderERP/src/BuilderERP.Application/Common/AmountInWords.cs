namespace BuilderERP.Application.Common;

public static class AmountInWords
{
    private static readonly string[] Ones =
    {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

    private static readonly string[] Tens =
    {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

    /// <summary>Converts an amount to BDT words, e.g. "Taka One Lakh Twenty Five Thousand and Fifty Paisa Only".</summary>
    public static string ToBdtWords(decimal amount)
    {
        var rounded = Math.Round(Math.Abs(amount), 2, MidpointRounding.AwayFromZero);
        var taka = (long)Math.Truncate(rounded);
        var paisa = (int)((rounded - taka) * 100);

        var result = "Taka " + (taka == 0 ? "Zero" : Convert(taka));
        if (paisa > 0)
        {
            result += $" and {Convert(paisa)} Paisa";
        }

        return (amount < 0 ? "Minus " : "") + result + " Only";
    }

    // South Asian grouping: Crore, Lakh, Thousand, Hundred.
    private static string Convert(long n)
    {
        var parts = new List<string>();

        void Add(long divisor, string name)
        {
            if (n >= divisor)
            {
                parts.Add($"{Convert(n / divisor)} {name}");
                n %= divisor;
            }
        }

        Add(10_000_000, "Crore");
        Add(100_000, "Lakh");
        Add(1_000, "Thousand");
        Add(100, "Hundred");

        if (n > 0)
        {
            parts.Add(n < 20 ? Ones[n] : Tens[n / 10] + (n % 10 > 0 ? " " + Ones[n % 10] : ""));
        }

        return string.Join(" ", parts);
    }
}
