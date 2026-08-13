namespace BuilderERP.Application.Features.Reports;

public class ReportRegistry
{
    private readonly IReadOnlyList<IReportSpec> _specs;

    public ReportRegistry(IEnumerable<IReportSpec> specs)
    {
        _specs = specs.OrderBy(s => s.Category).ThenBy(s => s.Name).ToList();
    }

    public IReadOnlyList<IGrouping<string, IReportSpec>> GetAllGroupedByCategory()
        => _specs.GroupBy(s => s.Category).ToList();

    public IReportSpec? GetByKey(string key)
        => _specs.FirstOrDefault(s => string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase));
}
