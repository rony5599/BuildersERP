using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.DefectRecords;
using BuilderERP.Application.Features.PropertyUnits;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.DefectRecordView)]
public class DefectRecordsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateDefectRecordDto> _createValidator;
    private readonly IValidator<UpdateDefectRecordDto> _updateValidator;

    public DefectRecordsController(IMediator mediator, IValidator<CreateDefectRecordDto> createValidator, IValidator<UpdateDefectRecordDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var items = await _mediator.Send(new GetAllDefectRecordsQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", items);
        }

        return View(items);
    }

    [PermissionAuthorize(PermissionNames.DefectRecordManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateDefectRecordDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DefectRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDefectRecordDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateDefectRecordCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.DefectRecordManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _mediator.Send(new GetDefectRecordByIdQuery(id));
        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDefectRecordDto
        {
            Id = item.Id,
            DefectNumber = item.DefectNumber,
            Description = item.Description,
            Category = item.Category,
            Severity = item.Severity,
            Status = item.Status,
            ReportedDate = item.ReportedDate,
            ResolvedDate = item.ResolvedDate,
            AssignedTo = item.AssignedTo,
            Remarks = item.Remarks,
            PropertyUnitId = item.PropertyUnitId,
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DefectRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDefectRecordDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateDefectRecordCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.DefectRecordManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetDefectRecordActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var propertyunits = await _mediator.Send(new GetAllPropertyUnitsQuery(PageSize: int.MaxValue));
        ViewBag.PropertyUnits = new SelectList(propertyunits.Items, "Id", "UnitNumber");
    }
}
