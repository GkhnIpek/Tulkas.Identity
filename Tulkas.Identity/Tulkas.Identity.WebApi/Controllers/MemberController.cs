using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

                string phone = _userManager.GetPhoneNumberAsync(CurrentUser).Result;
                if (phone != userViewModel.PhoneNumber)
                {
                    if (_userManager.Users.Any(u => u.PhoneNumber == userViewModel.PhoneNumber))
                    {
                        ModelState.AddModelError("", "Bu telefon numarası başka üye tarafından kayıtlıdır.");
                        return View(userViewModel);
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

        public IActionResult AccessDenied(string returnUrl)
        {
            if (returnUrl.Contains("ViolencePage"))
            {
                ViewBag.Message = "Erişim için 15 yaşınızdan büyük olmalısınız.";
            }
            else if (returnUrl.Contains("AnkaraPage"))
            {
                ViewBag.Message = "Bu sayfaya sadece şehir alanı Ankara olan kullanıcılar ulaşabilir.";
            }
            else if (returnUrl.Contains("Exchange"))
            {
                ViewBag.Message = "30 günlük ücretsiz deneme hakkınız sona ermiştir.";
            }
            else
            {
                ViewBag.Message = "Bu sayfaya erişim izniniz yoktur.";
            }
            return View();
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Editor()
        {
            return View();
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Manager()
        {
            return View();
        }

        [Authorize(policy: "AnkaraPolicy")]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(policy: "ViolencePolicy")]
        public IActionResult ViolencePage()
        {
            return View();
        }

        public async Task<IActionResult> ExchangeRedirect()
        {
            bool result = User.HasClaim(x => x.Type == "ExpireDateExchange");
            if (!result)
            {
                Claim ExpireDateExchange = new Claim("ExpireDateExchange", DateTime.Now.AddDays(30).Date.ToShortDateString(), ClaimValueTypes.String, "Internal");
                await _userManager.AddClaimAsync(CurrentUser, ExpireDateExchange);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(CurrentUser, true);
            }
            return RedirectToAction("Exchange");
        }

        [Authorize(policy: "ExchangePolicy")]
        public IActionResult Exchange()
        {
            return View();
        }
    }
}