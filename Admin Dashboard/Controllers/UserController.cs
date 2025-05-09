using Admin_Dashboard.ViewModels;
using AutoMapper;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin_Dashboard.Controllers
{
    public class UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var userViewModels = mapper.Map<IEnumerable<UserViewModel>>(users);
            return View(userViewModels);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);

            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                AvailableRoles = allRoles,
                SelectedRoles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            if (user.UserName != model.UserName)
            {
                var existingUser = await userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "This username is already taken.");

                    // Refresh roles for the view
                    model.AvailableRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }

                user.UserName = model.UserName;
                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    model.AvailableRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Except(model.SelectedRoles ?? new List<string>()).ToList();
            if (rolesToRemove.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    model.AvailableRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }
            }

            var rolesToAdd = (model.SelectedRoles ?? new List<string>()).Except(currentRoles).ToList();
            if (rolesToAdd.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    // Refresh roles for the view
                    model.AvailableRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }
            }

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);

            var userViewModel = mapper.Map<UserViewModel>(user);
            userViewModel.Roles = userRoles.ToList();

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index");
        }
    }
}
