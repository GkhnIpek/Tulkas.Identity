using Microsoft.AspNetCore.Identity;
using System;

namespace Tulkas.Identity.WebApi.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Gender { get; set; }
    }
}
