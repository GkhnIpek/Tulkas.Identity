using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;
using Tulkas.Identity.WebApi.Enums;
using Tulkas.Identity.WebApi.Models;
using Tulkas.Identity.WebApi.ViewModels;

namespace Tulkas.Identity.WebApi.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {
        }

        public IActionResult Index()
        {
            UserViewModel userViewModel = CurrentUser.Adapt<UserViewModel>();
            return View(userViewModel);
        }

        public IActionResult UserEdit()
        {
            UserViewModel userViewModel = CurrentUser.Adapt<UserViewModel>();
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        CurrentUser.Picture = "/img/" + fileName;
                    }
                }

                CurrentUser.UserName = userViewModel.UserName;
                CurrentUser.Email = userViewModel.Email;
                CurrentUser.PhoneNumber = userViewModel.PhoneNumber;
                CurrentUser.City = userViewModel.City;
                CurrentUser.BirthDate = userViewModel.BirthDate;
                CurrentUser.Gender = (int)userViewModel.Gender;

                IdentityResult result = await _userManager.UpdateAsync(CurrentUser);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(CurrentUser);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(CurrentUser, true);

                    ViewBag.Success = "true";
                }
                else
                {
                    AddModelError(result);
                }
            }

            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                bool exist = _userManager.CheckPasswordAsync(CurrentUser, passwordChangeViewModel.PasswordOld).Result;
                if (exist)
                {
                    IdentityResult result = _userManager.ChangePasswordAsync(CurrentUser,
                        passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew).Result;

                    if (result.Succeeded)
                    {
                        _userManager.UpdateSecurityStampAsync(CurrentUser);
                        _signInManager.SignOutAsync();
                        _signInManager.PasswordSignInAsync(CurrentUser, passwordChangeViewModel.PasswordNew, true, false);

                        ViewBag.Success = "true";
                    }
                    else
                    {
                        AddModelError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlıştır.");
                }
            }
            return View(passwordChangeViewModel);
        }

        public void Logout()
        {
            _signInManager.SignOutAsync();
        }
    }
}