using BuilderERP.Application.Features.Reports;
using BuilderERP.Application.Features.Reports.Export;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.ReportView)]
public class ReportsController : Controller
{
    private readonly ReportRegistry _registry;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ExcelReportExporter _excelExporter;
    private readonly PdfReportExporter _pdfExporter;

    public ReportsController(
        ReportRegistry registry,
        IUnitOfWork unitOfWork,
        ExcelReportExporter excelExporter,
        PdfReportExporter pdfExporter)
    {
        _registry = registry;
        _unitOfWork = unitOfWork;
        _excelExporter = excelExporter;
        _pdfExporter = pdfExporter;
    }

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
