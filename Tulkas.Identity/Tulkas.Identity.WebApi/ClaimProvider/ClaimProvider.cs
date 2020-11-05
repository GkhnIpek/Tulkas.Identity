﻿using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Tulkas.Identity.WebApi.Models;

namespace Tulkas.Identity.WebApi.ClaimProvider
{
    public class ClaimProvider : IClaimsTransformation
    {
        public UserManager<AppUser> _userManager { get; set; }

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                if (principal.Identity is ClaimsIdentity identity)
                {
                    AppUser user = await _userManager.FindByNameAsync(identity.Name);
                    if (user?.City != null)
                    {
                        if (!principal.HasClaim(c => c.Type == "City"))
                        {
                                Claim cityClaim = new Claim("City", user.City, ClaimValueTypes.String, "Internal");
                                identity.AddClaim(cityClaim);
                        }
                    }

                    if (user?.BirthDate != null)
                    {
                        var today = DateTime.Today;
                        var age = today.Year - user.BirthDate?.Year;
                        if (age > 15)
                        {
                            Claim violanceClaim = new Claim("Violence", true.ToString(), ClaimValueTypes.String, "Internal");
                            identity.AddClaim(violanceClaim);
                        }
                    }
                }
            }

            return principal;
        }
    }
}
