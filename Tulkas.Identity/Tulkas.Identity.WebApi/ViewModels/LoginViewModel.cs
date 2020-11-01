using System.ComponentModel.DataAnnotations;

namespace Tulkas.Identity.WebApi.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email adresi")]
        [Required(ErrorMessage = "Email alanı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter olmalıdır.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
