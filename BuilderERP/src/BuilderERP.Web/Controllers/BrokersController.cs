using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Brokers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.BrokerView)]
public class BrokersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateBrokerDto> _createValidator;
    private readonly IValidator<UpdateBrokerDto> _updateValidator;

    public BrokersController(IMediator mediator, IValidator<CreateBrokerDto> createValidator, IValidator<UpdateBrokerDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var brokers = await _mediator.Send(new GetAllBrokersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", brokers);
        }

        return View(brokers);
    }

    [PermissionAuthorize(PermissionNames.BrokerManage)]
    public IActionResult Create()
    {
        return View(new CreateBrokerDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BrokerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBrokerDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateBrokerCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.BrokerManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var broker = await _mediator.Send(new GetBrokerByIdQuery(id));
        if (broker is null)
        {
            return NotFound();
        }

        var dto = new UpdateBrokerDto
        {
            Id = broker.Id,
            Name = broker.Name,
            Phone = broker.Phone,
            Email = broker.Email,
            Address = broker.Address,
            LicenseNumber = broker.LicenseNumber,
            DefaultCommissionRate = broker.DefaultCommissionRate
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BrokerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBrokerDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateBrokerCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.BrokerManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetBrokerActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
