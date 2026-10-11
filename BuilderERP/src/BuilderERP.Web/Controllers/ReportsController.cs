using BuilderERP.Application.Features.Reports;
using BuilderERP.Application.Features.Reports.Export;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ReportView)]
public class ReportsController : Controller
{
    private static readonly IReadOnlyList<ReportColumn> ProcurementColumns = new[]
    {
        new ReportColumn("ProjectCode", "Project Code"), new ReportColumn("Project", "Project"),
        new ReportColumn("PRCount", "PO Req. Count"), new ReportColumn("PRAmount", "PO Req. Amount"),
        new ReportColumn("POCount", "PO Count"), new ReportColumn("POAmount", "PO Amount"),
        new ReportColumn("EWRCount", "EWO Req. Count"), new ReportColumn("EWRAmount", "EWO Req. Amount"),
        new ReportColumn("EWOCount", "EWO Count"), new ReportColumn("EWOAmount", "EWO Amount"),
        new ReportColumn("CRCount", "Cash PO Req. Count"), new ReportColumn("CRAmount", "Cash PO Req. Amount"),
        new ReportColumn("CPOCount", "Cash PO Count"), new ReportColumn("CPOAmount", "Cash PO Amount"),
        new ReportColumn("POBillCount", "PO Bill Count"), new ReportColumn("POBillAmount", "PO Bill Amount"),
        new ReportColumn("EWOBillCount", "EWO Bill Count"), new ReportColumn("EWOBillAmount", "EWO Bill Amount"),
        new ReportColumn("CPOBillCount", "Cash PO Bill Count"), new ReportColumn("CPOBillAmount", "Cash PO Bill Amount"),
        new ReportColumn("Committed", "Committed Total"), new ReportColumn("Billed", "Billed Total")
    };
    private readonly ReportRegistry _registry;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ExcelReportExporter _excelExporter;
    private readonly PdfReportExporter _pdfExporter;
    private readonly IMediator _mediator;

    public ReportsController(
        ReportRegistry registry,
        IUnitOfWork unitOfWork,
        ExcelReportExporter excelExporter,
        PdfReportExporter pdfExporter,
        IMediator mediator)
    {
        _registry = registry;
        _unitOfWork = unitOfWork;
        _excelExporter = excelExporter;
        _pdfExporter = pdfExporter;
        _mediator = mediator;
    }

    public async Task<IActionResult> ProcurementDashboard(long? projectId, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
    {
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
        {
            ModelState.AddModelError(string.Empty, "From date cannot be after To date.");
        }

        var model = await _mediator.Send(new GetProjectProcurementDashboardQuery(projectId, dateFrom, dateTo), cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshProcurementDashboard(long? projectId, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RefreshReportsCacheCommand(), cancellationToken);
        await _mediator.Send(new GetProjectProcurementDashboardQuery(projectId, dateFrom, dateTo)
        {
            BypassCacheRead = true
        }, cancellationToken);
        TempData["StatusMessage"] = "Report cache cleared. The dashboard now shows the latest data.";
        return RedirectToAction(nameof(ProcurementDashboard), new { projectId, dateFrom, dateTo });
    }

    public async Task<IActionResult> ExportProcurementDashboardExcel(long? projectId, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
    {
        var model = await _mediator.Send(new GetProjectProcurementDashboardQuery(projectId, dateFrom, dateTo), cancellationToken);
        return File(_excelExporter.Export("Project-wise Procurement Dashboard", ProcurementColumns, ToReportRows(model.Rows)),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "project-wise-procurement-dashboard.xlsx");
    }

    public async Task<IActionResult> ExportProcurementDashboardPdf(long? projectId, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
    {
        var model = await _mediator.Send(new GetProjectProcurementDashboardQuery(projectId, dateFrom, dateTo), cancellationToken);
        return File(_pdfExporter.Export("Project-wise Procurement Dashboard", ProcurementColumns, ToReportRows(model.Rows)),
            "application/pdf", "project-wise-procurement-dashboard.pdf");
    }

    private static IReadOnlyList<ReportRow> ToReportRows(IEnumerable<BuilderERP.Application.DTOs.ProjectProcurementRowDto> rows)
        => rows.Select(x => new ReportRow(new Dictionary<string, string?>
        {
            ["ProjectCode"] = x.ProjectCode, ["Project"] = x.ProjectName,
            ["PRCount"] = x.PurchaseRequisitionCount.ToString(), ["PRAmount"] = x.PurchaseRequisitionAmount.ToString("N2"),
            ["POCount"] = x.PurchaseOrderCount.ToString(), ["POAmount"] = x.PurchaseOrderAmount.ToString("N2"),
            ["EWRCount"] = x.EwoRequisitionCount.ToString(), ["EWRAmount"] = x.EwoRequisitionAmount.ToString("N2"),
            ["EWOCount"] = x.EwoCount.ToString(), ["EWOAmount"] = x.EwoAmount.ToString("N2"),
            ["CRCount"] = x.CashRequisitionCount.ToString(), ["CRAmount"] = x.CashRequisitionAmount.ToString("N2"),
            ["CPOCount"] = x.CashPurchaseOrderCount.ToString(), ["CPOAmount"] = x.CashPurchaseOrderAmount.ToString("N2"),
            ["POBillCount"] = x.PoBillCount.ToString(), ["POBillAmount"] = x.PoBillAmount.ToString("N2"),
            ["EWOBillCount"] = x.EwoBillCount.ToString(), ["EWOBillAmount"] = x.EwoBillAmount.ToString("N2"),
            ["CPOBillCount"] = x.CashPoBillCount.ToString(), ["CPOBillAmount"] = x.CashPoBillAmount.ToString("N2"),
            ["Committed"] = x.TotalCommittedAmount.ToString("N2"), ["Billed"] = x.TotalBilledAmount.ToString("N2")
        })).ToList();

    public IActionResult Index()
    {
        return View(_registry.GetAllGroupedByCategory());
    }

    public async Task<IActionResult> View(string key, [FromQuery] ReportFilter filter, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var spec = _registry.GetByKey(key);
        if (spec is null)
        {
            return NotFound();
        }

        if (page < 1) page = 1;
        if (pageSize is < 1 or > 200) pageSize = 25;

        var result = await spec.QueryAsync(_unitOfWork, filter, page, pageSize, cancellationToken);

        ViewBag.Spec = spec;
        ViewBag.Filter = filter;
        return View(result);
    }

    public async Task<IActionResult> ExportExcel(string key, [FromQuery] ReportFilter filter, CancellationToken cancellationToken)
    {
        var spec = _registry.GetByKey(key);
        if (spec is null)
        {
            return NotFound();
        }

        var rows = await spec.QueryAllAsync(_unitOfWork, filter, cancellationToken);
        var bytes = _excelExporter.Export(spec.Name, spec.Columns, rows);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{spec.Key}.xlsx");
    }

    public async Task<IActionResult> ExportPdf(string key, [FromQuery] ReportFilter filter, CancellationToken cancellationToken)
    {
        var spec = _registry.GetByKey(key);
        if (spec is null)
        {
            return NotFound();
        }

        var rows = await spec.QueryAllAsync(_unitOfWork, filter, cancellationToken);
        var bytes = _pdfExporter.Export(spec.Name, spec.Columns, rows);
        return File(bytes, "application/pdf", $"{spec.Key}.pdf");
    }
}
