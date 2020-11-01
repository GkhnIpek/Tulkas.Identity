using System.ComponentModel.DataAnnotations;

namespace Tulkas.Identity.WebApi.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Rol ismi")]
        [Required(ErrorMessage = "Rol ismi gereklidir.")]
        public string Name { get; set; }

    }
}
