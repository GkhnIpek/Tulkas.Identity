using System.ComponentModel.DataAnnotations;

namespace Tulkas.Identity.WebApi.ViewModels
{
    public class PasswordResetByAdminViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }
    }
}
