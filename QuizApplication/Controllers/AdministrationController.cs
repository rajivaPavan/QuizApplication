﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Entities;
using QuizApplication.Models;
using QuizApplication.ViewModels.RoleViewModels;

namespace QuizApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var userRole = new IdentityRole() {Name = model.RoleName};
            var result = await _roleManager.CreateAsync(userRole);

            if (result.Succeeded) return RedirectToAction("ListRoles", "Administration");

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in await _userManager.Users.ToListAsync())
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user.UserName);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null) return NotFound();

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded) return RedirectToAction("ListRoles");

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            var model = new List<UserRoleViewModel>();

            foreach (var user in await _userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userRoleViewModel.IsSelected = true;
                else
                    userRoleViewModel.IsSelected = false;

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(string roleId, List<UserRoleViewModel> model)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound();

            foreach (var userRoleViewModel in model)
            {
                var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);

                switch (userRoleViewModel.IsSelected)
                {
                    case true when !await _userManager.IsInRoleAsync(user, role.Name):
                        await _userManager.AddToRoleAsync(user, role.Name);
                        break;
                    case false when await _userManager.IsInRoleAsync(user, role.Name):
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                        break;
                    default:
                        continue;
                }
            }

            return RedirectToAction("EditRole", new {Id = roleId});
        }
    }
}