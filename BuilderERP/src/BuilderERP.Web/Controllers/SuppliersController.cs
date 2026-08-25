using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Suppliers;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.SupplierView)]
public class SuppliersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSupplierDto> _createValidator;
    private readonly IValidator<UpdateSupplierDto> _updateValidator;

    public SuppliersController(IMediator mediator, IValidator<CreateSupplierDto> createValidator, IValidator<UpdateSupplierDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var suppliers = await _mediator.Send(new GetAllSuppliersQuery(page, pageSize));

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", suppliers);
        }

        return View(suppliers);
    }

    [PermissionAuthorize(PermissionNames.SupplierManage)]
    public IActionResult Create()
    {
        return View(new CreateSupplierDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        await _mediator.Send(new CreateSupplierCommand(dto));
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.SupplierManage)]
    public async Task<IActionResult> Edit(long id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
        if (supplier is null)
        {
            return NotFound();
        }

        var dto = new UpdateSupplierDto
        {
            Id = supplier.Id,
            SupplierCode = supplier.SupplierCode,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            TaxRegistrationNumber = supplier.TaxRegistrationNumber,
            VatRegistrationNumber = supplier.VatRegistrationNumber,
            PaymentTerms = supplier.PaymentTerms,
            CreditLimit = supplier.CreditLimit,
            BankName = supplier.BankName,
            BankAccountNumber = supplier.BankAccountNumber,
            BankBranch = supplier.BankBranch,
            BankRoutingOrSwiftCode = supplier.BankRoutingOrSwiftCode,
            VendorCategory = supplier.VendorCategory,
            Rating = supplier.Rating
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSupplierDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var success = await _mediator.Send(new UpdateSupplierCommand(dto));
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.SupplierManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(long id, bool isActive)
    {
        await _mediator.Send(new SetSupplierActiveCommand(id, !isActive));
        return RedirectToAction(nameof(Index));
    }
}
