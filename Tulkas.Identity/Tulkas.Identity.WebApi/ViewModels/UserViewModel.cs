using System;
using System.ComponentModel.DataAnnotations;
using Tulkas.Identity.WebApi.Enums;

namespace Tulkas.Identity.WebApi.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Kullanıcı ismi gereklidir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Display(Name = "Tel. No")]
        [RegularExpression(@"^(0(\d{3}) (\d{3}) (\d{2}) (\d{2}))$", ErrorMessage = "Telefon numarası uygun formatta değil.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [Display(Name = "Email adresi")]
        [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değildir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime? BirthDate { get; set; }
        public string Picture { get; set; }

        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }
    }
}
