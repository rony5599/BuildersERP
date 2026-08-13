using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class ParkingSlotsReportSpec : IReportSpec
{
    public string Key => "parking-slots";
    public string Name => "Parking Slots Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("SlotNumber", "Slot #"),
        new ReportColumn("SlotType", "Type"),
        new ReportColumn("Project", "Project"),
        new ReportColumn("AllocatedTo", "Allocated To"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("CreatedAt", "Created"),
    };

    private static IQueryable<ParkingSlot> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<ParkingSlot>().Query().Include(x => x.Project).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.SlotNumber.Contains(f.Search) || (x.AllocatedTo != null && x.AllocatedTo.Contains(f.Search)));
        if (f.DateFrom.HasValue) q = q.Where(x => x.CreatedAt >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.CreatedAt <= f.DateTo);
        if (f.ProjectId.HasValue) q = q.Where(x => x.ProjectId == f.ProjectId);
        return q.OrderBy(x => x.SlotNumber);
    }

    public async Task<PagedResult<ReportRow>> QueryAsync(IUnitOfWork unitOfWork, ReportFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var q = BaseQuery(unitOfWork, filter);
        var total = await q.CountAsync(cancellationToken);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<ReportRow>(items.Select(ToRow).ToList(), total, page, pageSize);
    }

    public async Task<IReadOnlyList<ReportRow>> QueryAllAsync(IUnitOfWork unitOfWork, ReportFilter filter, CancellationToken cancellationToken)
    {
        var items = await BaseQuery(unitOfWork, filter).Take(10_000).ToListAsync(cancellationToken);
        return items.Select(ToRow).ToList();
    }

    private static ReportRow ToRow(ParkingSlot x) => new(new Dictionary<string, string?>
    {
        ["SlotNumber"] = x.SlotNumber,
        ["SlotType"] = x.SlotType.ToString(),
        ["Project"] = x.Project?.Name,
        ["AllocatedTo"] = x.AllocatedTo,
        ["Status"] = x.Status.ToString(),
        ["CreatedAt"] = x.CreatedAt.ToString("yyyy-MM-dd"),
    });
}
