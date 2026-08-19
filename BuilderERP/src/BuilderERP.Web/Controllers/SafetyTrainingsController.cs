using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.SafetyTrainings;
using BuilderERP.Application.Features.Workers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SafetyTrainingView)]
public class SafetyTrainingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSafetyTrainingDto> _createValidator;
    private readonly IValidator<UpdateSafetyTrainingDto> _updateValidator;

    public SafetyTrainingsController(IMediator mediator, IValidator<CreateSafetyTrainingDto> createValidator, IValidator<UpdateSafetyTrainingDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var trainings = await _mediator.Send(new GetAllSafetyTrainingsQuery(Page: page, PageSize: pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", trainings);
        }

        return View(trainings);
    }

    [PermissionAuthorize(PermissionNames.SafetyTrainingManage)]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateSafetyTrainingDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyTrainingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSafetyTrainingDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        await _mediator.Send(new CreateSafetyTrainingCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SafetyTrainingManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var training = await _mediator.Send(new GetSafetyTrainingByIdQuery(id));
        if (training is null)
        {
            return NotFound();
        }

        var dto = new UpdateSafetyTrainingDto
        {
            Id = training.Id,
            WorkerId = training.WorkerId,
            TrainingTitle = training.TrainingTitle,
            TrainingDate = training.TrainingDate,
            TrainerName = training.TrainerName,
            DurationHours = training.DurationHours,
            CertificateNumber = training.CertificateNumber,
            ExpiryDate = training.ExpiryDate
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyTrainingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSafetyTrainingDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSafetyTrainingCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SafetyTrainingManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
    {
        await _mediator.Send(new SetSafetyTrainingActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var workers = await _mediator.Send(new GetAllWorkersQuery(PageSize: int.MaxValue));
        ViewBag.Workers = new SelectList(workers.Items, "Id", "Name");
    }
}
