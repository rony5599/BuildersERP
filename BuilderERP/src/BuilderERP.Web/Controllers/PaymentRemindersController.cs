using BuilderERP.Application.Features.PaymentReminders;
using BuilderERP.Domain.Enums;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.PaymentReminderView)]
public class PaymentRemindersController : Controller
{
    private readonly IMediator _mediator;

    public PaymentRemindersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(ReminderStatus? status, int page = 1, int pageSize = 25)
    {
        var reminders = await _mediator.Send(new GetAllPaymentRemindersQuery(status, page, pageSize));
        ViewBag.SelectedStatus = status;

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", reminders);
        }

        return View(reminders);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PaymentReminderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(int lookaheadDays = 7)
    {
        var created = await _mediator.Send(new GenerateRemindersCommand(lookaheadDays));
        TempData["ReminderGenerationResult"] = $"{created} reminder(s) generated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PaymentReminderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkSent(Guid id, ReminderChannel channel)
    {
        await _mediator.Send(new MarkReminderSentCommand(id, channel));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.PaymentReminderManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _mediator.Send(new CancelReminderCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
