namespace BuilderERP.Application.Features.Reports;

public class ReportRow
{
    public ReportRow(IReadOnlyDictionary<string, string?> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<string, string?> Cells { get; }
}
