using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IdentitySample.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;

namespace IdentitySample.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ManageRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public ManageRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            { }
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var role = new IdentityRole(model.Name);
            var result = await _roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "مشکلی پیش آمده است.");
            return View(model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();
            var role =await _roleManager.FindByIdAsync(id.ToString());
            if(role==null)
                return NotFound();
            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DoesRoleExist(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role!=null)
            {
                return Json("دسترسی وارد شده در سایت موجود است.");
            }
            return Json(true);
        }
    }
}

