namespace BuilderERP.Application.Features.Reports;

public class ReportFilter
{
    public string? Search { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? ProjectId { get; set; }
}
