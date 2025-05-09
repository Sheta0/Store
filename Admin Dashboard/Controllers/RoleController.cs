using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Admin_Dashboard.Controllers
{
    public class RoleController(RoleManager<IdentityRole> roleManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new RoleFormViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var roleExists = await roleManager.RoleExistsAsync(model.Name);
            if (roleExists)
            {
                ModelState.AddModelError("Name", "Role already exists.");
                return View(model);
            }

            await roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));
            TempData["SuccessMessage"] = "Role created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction("Index");
            }

            await roleManager.DeleteAsync(role);
            TempData["SuccessMessage"] = "Role deleted successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var mappedRole = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(mappedRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.Name != model.Name)
            {
                var roleWithSameName = await roleManager.FindByNameAsync(model.Name);
                if (roleWithSameName != null)
                {
                    ModelState.AddModelError("Name", "Role with this name already exists.");
                    return View(model);
                }
            }

            role.Name = model.Name.Trim();
            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role updated successfully.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
