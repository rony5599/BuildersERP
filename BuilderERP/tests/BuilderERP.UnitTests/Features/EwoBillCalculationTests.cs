using BuilderERP.Application.Features.EwoBills;
using Xunit;

namespace BuilderERP.UnitTests.Features;

public class EwoBillCalculationTests
{
    [Fact]
    public void First_bill_pays_measured_total_times_claimed_heads()
    {
        // Contractor bill 01: work done 275,080; Vertical 18% + Internal 22%.
        var bill = EwoBillBuilder.Calculate(275_080m, 0, 40, 0, 0, 0);

        Assert.Equal(40, bill.CumulativePercent);
        Assert.Equal(110_032m, bill.CumulativeDue);
        Assert.Equal(110_032m, bill.CertifiedAmount);
        Assert.Equal(110_032m, bill.NetPayable);
    }

    [Fact]
    public void Later_bill_is_cumulative_and_corrects_earlier_heads_for_remeasurement()
    {
        // Bill 02 re-measures to 290,000 and claims RCC 15% + Roof 15%:
        // 290,000 x 70% - 110,032 = 92,968 (87,000 for new heads + 5,968 catch-up on Vertical/Internal).
        var bill = EwoBillBuilder.Calculate(290_000m, 40, 30, 110_032m, 0, 0);

        Assert.Equal(70, bill.CumulativePercent);
        Assert.Equal(203_000m, bill.CumulativeDue);
        Assert.Equal(92_968m, bill.CertifiedAmount);
        Assert.Equal(92_968m, bill.NetPayable);
    }

    [Fact]
    public void Partial_head_claim_is_a_share_of_the_contract_percent()
    {
        // Half of the 20% Finishing head.
        var bill = EwoBillBuilder.Calculate(100_000m, 0, 10, 0, 0, 0);

        Assert.Equal(10_000m, bill.NetPayable);
    }

    [Fact]
    public void Additions_and_deductions_adjust_only_this_bill()
    {
        var bill = EwoBillBuilder.Calculate(275_080m, 0, 40, 0, additions: 1_000m, deductions: 5_501.60m);

        Assert.Equal(110_032m, bill.CertifiedAmount);
        Assert.Equal(1_000m, bill.AdditionAmount);
        Assert.Equal(5_501.60m, bill.DeductionAmount);
        Assert.Equal(105_530.40m, bill.NetPayable);
    }

    [Fact]
    public void Cumulative_due_is_rounded_to_two_decimals()
    {
        var bill = EwoBillBuilder.Calculate(1_234.57m, 0, 18, 0, 0, 0);

        Assert.Equal(222.22m, bill.CumulativeDue);
    }
}
