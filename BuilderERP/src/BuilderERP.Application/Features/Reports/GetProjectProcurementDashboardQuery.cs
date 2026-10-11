using BuilderERP.Application.DTOs;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Reports;

public record GetProjectProcurementDashboardQuery(long? ProjectId = null, DateTime? DateFrom = null, DateTime? DateTo = null)
    : IRequest<ProjectProcurementDashboardDto>, IBypassesCacheRead
{
    // Not part of the positional record state, so refresh writes back to the
    // exact same cache key used by ordinary dashboard requests.
    [System.Text.Json.Serialization.JsonIgnore]
    public bool BypassCacheRead { get; init; }
}

public class GetProjectProcurementDashboardQueryHandler
    : IRequestHandler<GetProjectProcurementDashboardQuery, ProjectProcurementDashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectProcurementDashboardQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ProjectProcurementDashboardDto> Handle(GetProjectProcurementDashboardQuery request, CancellationToken ct)
    {
        var from = request.DateFrom?.Date;
        var toExclusive = request.DateTo?.Date.AddDays(1);
        var projects = await _unitOfWork.Repository<Project>().Query()
            .Where(x => x.IsActive && (!request.ProjectId.HasValue || x.Id == request.ProjectId.Value))
            .OrderBy(x => x.Name)
            .Select(x => new ProjectProcurementRowDto { ProjectId = x.Id, ProjectCode = x.Code, ProjectName = x.Name })
            .ToListAsync(ct);
        var rows = projects.ToDictionary(x => x.ProjectId);

        // Aggregate from BOQ items rather than nesting an item SUM inside a header SUM.
        // SQL Server does not allow an aggregate over an expression containing another aggregate.
        var boqBudgetItems = _unitOfWork.Repository<BoqItem>().Query()
            .Where(x => x.Boq.IsActive && x.Boq.Status == BoqStatus.Approved);
        if (request.ProjectId.HasValue) boqBudgetItems = boqBudgetItems.Where(x => x.Boq.ProjectId == request.ProjectId.Value);
        foreach (var x in await boqBudgetItems.GroupBy(x => x.Boq.ProjectId)
                     .Select(g => new
                     {
                         Id = g.Key,
                         Amount = g.Sum(x => x.Quantity * x.Rate * (1m + x.Boq.ContingencyPercent / 100m))
                     }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) row.BoqBudgetAmount = x.Amount;

        var pr = _unitOfWork.Repository<PurchaseRequisition>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) pr = pr.Where(x => x.ProjectId == request.ProjectId.Value);
        if (from.HasValue) pr = pr.Where(x => x.RequestDate >= from.Value);
        if (toExclusive.HasValue) pr = pr.Where(x => x.RequestDate < toExclusive.Value);
        foreach (var x in await pr.GroupBy(x => x.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.EstimatedAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.PurchaseRequisitionCount = x.Count; row.PurchaseRequisitionAmount = x.Amount; }

        var po = _unitOfWork.Repository<PurchaseOrder>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) po = po.Where(x => x.VendorQuotation.Rfq.PurchaseRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) po = po.Where(x => x.OrderDate >= from.Value);
        if (toExclusive.HasValue) po = po.Where(x => x.OrderDate < toExclusive.Value);
        foreach (var x in await po.GroupBy(x => x.VendorQuotation.Rfq.PurchaseRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.PurchaseOrderCount = x.Count; row.PurchaseOrderAmount = x.Amount; }

        var ewr = _unitOfWork.Repository<EngineerWorkOrderRequisition>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) ewr = ewr.Where(x => x.ProjectId == request.ProjectId.Value);
        if (from.HasValue) ewr = ewr.Where(x => x.RequestDate >= from.Value);
        if (toExclusive.HasValue) ewr = ewr.Where(x => x.RequestDate < toExclusive.Value);
        foreach (var x in await ewr.GroupBy(x => x.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.EstimatedAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.EwoRequisitionCount = x.Count; row.EwoRequisitionAmount = x.Amount; }

        var ewo = _unitOfWork.Repository<EngineerWorkOrder>().Query().Where(x => x.IsLatestRevision);
        if (request.ProjectId.HasValue) ewo = ewo.Where(x => x.EngineerWorkOrderRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) ewo = ewo.Where(x => x.CreatedAt >= from.Value);
        if (toExclusive.HasValue) ewo = ewo.Where(x => x.CreatedAt < toExclusive.Value);
        foreach (var x in await ewo.GroupBy(x => x.EngineerWorkOrderRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.EwoCount = x.Count; row.EwoAmount = x.Amount; }

        var cr = _unitOfWork.Repository<CashRequisition>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) cr = cr.Where(x => x.ProjectId == request.ProjectId.Value);
        if (from.HasValue) cr = cr.Where(x => x.RequestDate >= from.Value);
        if (toExclusive.HasValue) cr = cr.Where(x => x.RequestDate < toExclusive.Value);
        foreach (var x in await cr.GroupBy(x => x.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.EstimatedAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.CashRequisitionCount = x.Count; row.CashRequisitionAmount = x.Amount; }

        var cpo = _unitOfWork.Repository<CashPurchaseOrder>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) cpo = cpo.Where(x => x.CashRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) cpo = cpo.Where(x => x.OrderDate >= from.Value);
        if (toExclusive.HasValue) cpo = cpo.Where(x => x.OrderDate < toExclusive.Value);
        foreach (var x in await cpo.GroupBy(x => x.CashRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.CashPurchaseOrderCount = x.Count; row.CashPurchaseOrderAmount = x.Amount; }

        var monthlyCommitments = new Dictionary<(int Year, int Month), MonthlyProcurementCommitmentDto>();
        foreach (var x in await po.GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
                     .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            GetMonth(x.Year, x.Month).PurchaseOrderAmount = x.Amount;
        foreach (var x in await ewo.GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                     .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            GetMonth(x.Year, x.Month).EwoAmount = x.Amount;
        foreach (var x in await cpo.GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
                     .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            GetMonth(x.Year, x.Month).CashPurchaseOrderAmount = x.Amount;

        MonthlyProcurementCommitmentDto GetMonth(int year, int month)
        {
            var key = (year, month);
            if (!monthlyCommitments.TryGetValue(key, out var value))
            {
                value = new MonthlyProcurementCommitmentDto { Year = year, Month = month };
                monthlyCommitments[key] = value;
            }
            return value;
        }

        var poBills = _unitOfWork.Repository<PoBill>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) poBills = poBills.Where(x => x.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) poBills = poBills.Where(x => x.BillDate >= from.Value);
        if (toExclusive.HasValue) poBills = poBills.Where(x => x.BillDate < toExclusive.Value);
        foreach (var x in await poBills.GroupBy(x => x.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.PoBillCount = x.Count; row.PoBillAmount = x.Amount; }

        var ewoBills = _unitOfWork.Repository<EwoBill>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) ewoBills = ewoBills.Where(x => x.EngineerWorkOrder.EngineerWorkOrderRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) ewoBills = ewoBills.Where(x => x.BillDate >= from.Value);
        if (toExclusive.HasValue) ewoBills = ewoBills.Where(x => x.BillDate < toExclusive.Value);
        foreach (var x in await ewoBills.GroupBy(x => x.EngineerWorkOrder.EngineerWorkOrderRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.NetPayable) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.EwoBillCount = x.Count; row.EwoBillAmount = x.Amount; }

        var cashPoBills = _unitOfWork.Repository<CashPoBill>().Query().Where(x => x.IsActive);
        if (request.ProjectId.HasValue) cashPoBills = cashPoBills.Where(x => x.CashPurchaseOrder.CashRequisition.ProjectId == request.ProjectId.Value);
        if (from.HasValue) cashPoBills = cashPoBills.Where(x => x.BillDate >= from.Value);
        if (toExclusive.HasValue) cashPoBills = cashPoBills.Where(x => x.BillDate < toExclusive.Value);
        foreach (var x in await cashPoBills.GroupBy(x => x.CashPurchaseOrder.CashRequisition.ProjectId).Select(g => new { Id = g.Key, Count = g.Count(), Amount = g.Sum(x => x.TotalAmount) }).ToListAsync(ct))
            if (rows.TryGetValue(x.Id, out var row)) { row.CashPoBillCount = x.Count; row.CashPoBillAmount = x.Amount; }

        return new ProjectProcurementDashboardDto
        {
            ProjectId = request.ProjectId, DateFrom = request.DateFrom, DateTo = request.DateTo,
            Projects = await _unitOfWork.Repository<Project>().Query().Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new ProjectLookupDto { Id = x.Id, Name = x.Code + " - " + x.Name }).ToListAsync(ct),
            Rows = projects,
            MonthlyCommitments = monthlyCommitments.Values.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList()
        };
    }
}
