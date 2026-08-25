using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Shared.Authorization;
using BuilderERP.Shared.Constants;
using BuilderERP.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Web.Controllers;

[PermissionAuthorize(PermissionNames.RoleView)]
public class RolesController : Controller
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRoleDto> _createValidator;
    private readonly IValidator<UpdateRoleDto> _updateValidator;

    public RolesController(
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork,
        IValidator<CreateRoleDto> createValidator,
        IValidator<UpdateRoleDto> updateValidator)
    {
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public IActionResult Index(int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name ?? string.Empty, Description = r.Description });

        var totalCount = query.Count();
        var roles = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<RoleDto>(roles, totalCount, page, pageSize);

        if (this.IsAjaxRequest())
        {
            return PartialView("_Grid", result);
        }

        return View(result);
    }

    [PermissionAuthorize(PermissionNames.RoleManage)]
    public IActionResult Create()
    {
        return View(new CreateRoleDto());
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RoleManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = dto.Name, Description = dto.Description });
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RoleManage)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        return View(new UpdateRoleDto { Id = role.Id, Name = role.Name ?? string.Empty, Description = role.Description });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RoleManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRoleDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(dto);
        }

        var role = await _roleManager.FindByIdAsync(dto.Id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        role.Name = dto.Name;
        role.Description = dto.Description;
        await _roleManager.UpdateAsync(role);

        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize(PermissionNames.RoleManage)]
    public async Task<IActionResult> Permissions(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        var allPermissions = await _unitOfWork.Repository<Permission>().GetAllAsync();
        var grantedIds = await _unitOfWork.Repository<RolePermission>()
            .Find(rp => rp.RoleId == role.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var groups = allPermissions
            .GroupBy(p => p.Module)
            .OrderBy(g => g.Key)
            .Select(g => new PermissionGroupDto
            {
                Module = g.Key,
                Permissions = g.OrderBy(p => p.Name)
                    .Select(p => new PermissionItemDto { Id = p.Id, Name = p.Name, IsGranted = grantedIds.Contains(p.Id) })
                    .ToList()
            })
            .ToList();

        return View(new RolePermissionsDto { RoleId = role.Id, RoleName = role.Name ?? string.Empty, Groups = groups });
    }

    [HttpPost]
    [PermissionAuthorize(PermissionNames.RoleManage)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(Guid id, List<long> selectedPermissionIds)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        var repository = _unitOfWork.Repository<RolePermission>();
        var existingGrants = await repository.Find(rp => rp.RoleId == role.Id).ToListAsync();

        foreach (var grant in existingGrants.Where(g => !selectedPermissionIds.Contains(g.PermissionId)))
        {
            repository.Remove(grant);
        }

        var existingPermissionIds = existingGrants.Select(g => g.PermissionId).ToHashSet();
        foreach (var permissionId in selectedPermissionIds.Where(pid => !existingPermissionIds.Contains(pid)))
        {
            await repository.AddAsync(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
        }

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
