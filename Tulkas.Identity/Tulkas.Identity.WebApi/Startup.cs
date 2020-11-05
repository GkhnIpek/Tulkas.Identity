using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tulkas.Identity.WebApi.CustomValidation;
using Tulkas.Identity.WebApi.Models;

namespace Tulkas.Identity.WebApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();

            services.AddDbContext<AppIdentityDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AnkaraPolicy", policy =>
                {
                    policy.RequireClaim("City", "Ankara");
                });

                options.AddPolicy("ViolencePolicy", policy =>
                {
                    policy.RequireClaim("Violence");
                });

                options.AddPolicy("ExchangePolicy", policy =>
                {
                    policy.AddRequirements(new ExpireDateExchangeRequirement());
                });
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnopqrsþtuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }).AddPasswordValidator<CustomPasswordValidator>()
               .AddUserValidator<CustomUserValidator>()
               .AddErrorDescriber<CustomIdentityErrorDescribe>()
               .AddEntityFrameworkStores<AppIdentityDbContext>()
               .AddDefaultTokenProviders();

            CookieBuilder cookieBuilder = new CookieBuilder
            {
                Name = "MyBlog",
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                SecurePolicy = CookieSecurePolicy.SameAsRequest
            };

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.LoginPath = new PathString("/Home/Login");
                options.LogoutPath = new PathString("/Member/Logout");
                options.Cookie = cookieBuilder;
                options.SlidingExpiration = true;
                options.AccessDeniedPath = new PathString("/Member/AccessDenied");
            });

            services.AddScoped<IClaimsTransformation, ClaimProvider.ClaimProvider>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
