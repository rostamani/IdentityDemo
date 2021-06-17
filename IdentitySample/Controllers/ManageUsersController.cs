using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IdentitySample.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using IdentitySample.Repositories;
using System.Security.Claims;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUsersController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users.Select(u => new IndexViewModel
            {
                UserId = u.Id,
                Email = u.Email,
                Username = u.UserName
            }).ToList());
        }

        public async Task<IActionResult> Edit(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var claims = ClaimStore.Claims.Select(c => new ClaimViewModel
            {
                ClaimType = c.Type,
                IsSelected = false
            }).ToList();
            var userClaims =(await  _userManager.GetClaimsAsync(user)).Select(uc=>uc.Type);
            
            foreach (var claim in claims)
            {
                if(userClaims.Contains(claim.ClaimType))
                {
                    claim.IsSelected = true;
                }
            }
            var roles = _roleManager.Roles.Select(role=>new RoleViewModel { 
                Name=role.Name,
                IsSelected=false
            }).ToList();
            foreach (var item in roles)
            {
                if(await _userManager.IsInRoleAsync(user,item.Name))
                {
                    item.IsSelected = true;
                }
            }
            var model = new EditViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles,
                Claims=claims.ToList()
            };
            //return View(_mapper.Map<IndexViewModel>(user));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);

            var requestedRoles = model.Roles.Where(role => role.IsSelected).Select(role => role.Name).ToList();
            var requestedClaims = model.Claims.Where(claim => claim.IsSelected).Select(claim => new Claim(claim.ClaimType, true.ToString())).ToList();

            var userRoles = await _userManager.GetRolesAsync(user);
            var shouldBeDeletedRoles = userRoles.Where(u => !requestedRoles.Contains(u)).ToList();
            var shouldBeAddedRoles = requestedRoles.Where(u => !userRoles.Contains(u)).ToList();

            var userClaims = (await _userManager.GetClaimsAsync(user))/*.Select(c=>c.Type).ToList()*/;
            var shouldBeDeletedClaims = userClaims.Where(c => !requestedClaims.Contains(c)).ToList();
            var shouldBeAddedClaims = requestedClaims.Where(c => !userClaims.Contains(c)).ToList();

            user.UserName = model.Username;
            user.Email = model.Email;
            await _userManager.RemoveFromRolesAsync(user, shouldBeDeletedRoles);
            await _userManager.AddToRolesAsync(user, shouldBeAddedRoles);
            await _userManager.RemoveClaimsAsync(user, shouldBeDeletedClaims);
            await _userManager.AddClaimsAsync(user, shouldBeAddedClaims);
            var result =await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "مشکلی رخ داد");
            return View(model);
        }

        //public async Task<IActionResult> EditRoles(string userId)
        //{
        //    if(userId==null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);

        //    if(user==null)
        //    {
        //        return NotFound();
        //    }
            
        //}
    }
}
