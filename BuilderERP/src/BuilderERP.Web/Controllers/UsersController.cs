using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Companies;
using BuilderERP.Domain.Entities;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using BuilderERP.Web.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.UserView)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMediator mediator,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _userManager.Users
            .Include(u => u.Company)
            .Include(u => u.Branch)
            .OrderBy(u => u.Email);

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CompanyName = user.Company?.Name,
                BranchName = user.Branch?.Name,
                Roles = roles.ToList()
            });
        }

        var result = new PagedResult<UserDto>(dtos, totalCount, page, pageSize);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", result);
        }

        return View(result);
    }

    [PermissionAuthorize(PermissionNames.UserManage)]
    public async Task<IActionResult> Create(string? returnUrl = null)
    {
        await PopulateDropdownsAsync();
        ViewBag.ReturnUrl = returnUrl;
        return View(new CreateUserDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.UserManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto, string? returnUrl = null)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true,
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await PopulateDropdownsAsync();
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        await _userManager.AddToRoleAsync(user, dto.Role);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.UserManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var dto = new UpdateUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            CompanyId = user.CompanyId,
            BranchId = user.BranchId,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.UserManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateUserDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        user.FullName = dto.FullName;
        user.CompanyId = dto.CompanyId;
        user.BranchId = dto.BranchId;
        user.IsActive = dto.IsActive;
        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(dto.Role))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var model = new ResetPasswordViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        model.Email = user.Email ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        TempData["StatusMessage"] = $"Password for {user.Email} has been reset.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var companies = await _mediator.Send(new GetAllCompaniesQuery(PageSize: int.MaxValue));
        ViewBag.Companies = new SelectList(companies.Items, "Id", "Name");

        var branches = await _mediator.Send(new BuilderERP.Application.Features.Branches.GetAllBranchesQuery(PageSize: int.MaxValue));
        ViewBag.Branches = new SelectList(branches.Items, "Id", "Name");

        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        ViewBag.Roles = new SelectList(roles);
    }
}
