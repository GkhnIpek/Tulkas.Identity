using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tulkas.Identity.WebApi.Models;

namespace Tulkas.Identity.WebApi.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> _userManager { get; set; }
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
    }
}
