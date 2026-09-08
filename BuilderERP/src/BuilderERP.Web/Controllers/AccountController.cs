using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Identity;
using BuilderERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDeviceRecognitionService _deviceRecognition;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IDeviceRecognitionService deviceRecognition,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _deviceRecognition = deviceRecognition;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        var deviceId = _deviceRecognition.GetOrCreateDeviceId(HttpContext);
        var deviceStatus = await _deviceRecognition.CheckDeviceAsync(user.Id, deviceId, HttpContext);

        if (deviceStatus == DeviceStatus.Pending)
        {
            _logger.LogInformation("Login blocked for {Email}: device {DeviceId} is pending approval", model.Email, deviceId);
            return View("DeviceApprovalRequired", model);
        }

        if (deviceStatus != DeviceStatus.Approved)
        {
            _logger.LogInformation("Login blocked for {Email}: device {DeviceId} status is {Status}", model.Email, deviceId, deviceStatus);
            return AddDeviceError(model, deviceStatus);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in", model.Email);
            await _deviceRecognition.RecordSuccessfulLoginAsync(user.Id, deviceId, HttpContext);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account has been locked out. Please try again later.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    private IActionResult AddDeviceError(LoginViewModel model, DeviceStatus deviceStatus)
    {
        var message = deviceStatus switch
        {
            DeviceStatus.Rejected => "This device was rejected by an administrator.",
            DeviceStatus.Revoked => "Access from this device has been revoked.",
            DeviceStatus.Blocked => "This device has been blocked.",
            _ => "This device is not approved for login."
        };

        ModelState.AddModelError(string.Empty, message);
        return View("Login", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User {Email} changed their password", user.Email);

        TempData["StatusMessage"] = "Your password has been changed successfully.";
        return RedirectToAction("ChangePassword");
    }
}
