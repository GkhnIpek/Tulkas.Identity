using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tulkas.Identity.WebApi.Models;
using Tulkas.Identity.WebApi.ViewModels;

namespace Tulkas.Identity.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
            : base(userManager, null, roleManager)
        {
        }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        public IActionResult Claims()
        {
            return View(User.Claims.ToList());
        }

        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            AppRole role = new AppRole { Name = roleViewModel.Name };
            IdentityResult result = _roleManager.CreateAsync(role).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("GetRoles");
            }

            AddModelError(result);
            return View(roleViewModel);
        }

        public IActionResult GetRoles()
        {
            return View(_roleManager.Roles.ToList());
        }

        [HttpPost]
        public IActionResult RoleDelete(string id)
        {
            AppRole role = _roleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                IdentityResult result = _roleManager.DeleteAsync(role).Result;
            }

            return RedirectToAction("GetRoles");
        }

        public IActionResult RoleUpdate(string id)
        {
            AppRole role = _roleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                return View(role.Adapt<RoleViewModel>());
            }

            return RedirectToAction("GetRoles");
        }

        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel)
        {
            AppRole role = _roleManager.FindByIdAsync(roleViewModel.Id).Result;
            if (role != null)
            {
                role.Name = roleViewModel.Name;
                IdentityResult result = _roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu.");
            }

            return View(roleViewModel);
        }

        public IActionResult RoleAssign(string id)
        {
            TempData["userId"] = id;
            AppUser user = _userManager.FindByIdAsync(id).Result;
            ViewBag.UserName = user.UserName;

            IQueryable<AppRole> roles = _roleManager.Roles;
            List<string> userRoles = _userManager.GetRolesAsync(user).Result as List<string>;

            List<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>();
            foreach (var role in roles)
            {
                RoleAssignViewModel r = new RoleAssignViewModel();;
                r.RoleId = role.Id;
                r.RoleName = role.Name;
                if (userRoles != null) r.Exist = userRoles.Contains(role.Name);

                roleAssignViewModels.Add(r);
            }
            return View(roleAssignViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels)
        {
            AppUser user = _userManager.FindByIdAsync(TempData["userId"].ToString()).Result;
            foreach (var roleAssignViewModel in roleAssignViewModels)
            {
                if (roleAssignViewModel.Exist)
                {
                    await _userManager.AddToRoleAsync(user, roleAssignViewModel.RoleName);
                }
                else
                {
                    await  _userManager.RemoveFromRoleAsync(user, roleAssignViewModel.RoleName);
                }
            }

            return RedirectToAction("Users");
        }
    }
}
