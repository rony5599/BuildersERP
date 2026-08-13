using BuilderERP.Application.Common;
using BuilderERP.Domain.Interfaces;

namespace BuilderERP.Application.Features.Reports;

public interface IReportSpec
{
    string Key { get; }
    string Name { get; }
    string Category { get; }
    IReadOnlyList<ReportColumn> Columns { get; }

    Task<PagedResult<ReportRow>> QueryAsync(IUnitOfWork unitOfWork, ReportFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyList<ReportRow>> QueryAllAsync(IUnitOfWork unitOfWork, ReportFilter filter, CancellationToken cancellationToken);
}
