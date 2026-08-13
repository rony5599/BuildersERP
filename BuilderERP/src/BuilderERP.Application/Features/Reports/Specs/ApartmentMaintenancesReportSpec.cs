using System.Globalization;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports.Specs;

public class ApartmentMaintenancesReportSpec : IReportSpec
{
    public string Key => "apartment-maintenances";
    public string Name => "Apartment Maintenances Report";
    public string Category => "Facility Management";

    public IReadOnlyList<ReportColumn> Columns => new[]
    {
        new ReportColumn("MaintenanceNumber", "Maintenance #"),
        new ReportColumn("PropertyUnit", "Unit"),
        new ReportColumn("MaintenanceType", "Type"),
        new ReportColumn("ScheduledDate", "Scheduled"),
        new ReportColumn("CompletedDate", "Completed"),
        new ReportColumn("Cost", "Cost"),
        new ReportColumn("Status", "Status"),
        new ReportColumn("AssignedTo", "Assigned To"),
    };

    private static IQueryable<ApartmentMaintenance> BaseQuery(IUnitOfWork uow, ReportFilter f)
    {
        var q = uow.Repository<ApartmentMaintenance>().Query().Include(x => x.PropertyUnit).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(x => x.MaintenanceNumber.Contains(f.Search) || x.PropertyUnit.UnitNumber.Contains(f.Search));
        if (f.DateFrom.HasValue) q = q.Where(x => x.ScheduledDate >= f.DateFrom);
        if (f.DateTo.HasValue) q = q.Where(x => x.ScheduledDate <= f.DateTo);
        return q.OrderByDescending(x => x.ScheduledDate);
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

    private static ReportRow ToRow(ApartmentMaintenance x) => new(new Dictionary<string, string?>
    {
        ["MaintenanceNumber"] = x.MaintenanceNumber,
        ["PropertyUnit"] = x.PropertyUnit?.UnitNumber,
        ["MaintenanceType"] = x.MaintenanceType.ToString(),
        ["ScheduledDate"] = x.ScheduledDate.ToString("yyyy-MM-dd"),
        ["CompletedDate"] = x.CompletedDate?.ToString("yyyy-MM-dd"),
        ["Cost"] = x.Cost.ToString("N2", CultureInfo.InvariantCulture),
        ["Status"] = x.Status.ToString(),
        ["AssignedTo"] = x.AssignedTo,
    });
}
