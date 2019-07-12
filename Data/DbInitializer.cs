using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Data
{
    public class DbInitializer
    {
        public static async Task InitializAsync(ApplicationDbContext context, IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Cashier ", "SuperAdmin" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));

                }
                string email = "demo@multiskills.com";
                string password = "admin@123";

                if (userManager.FindByEmailAsync(email).Result == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.UserName = email;
                    IdentityResult result = userManager.CreateAsync(user, password).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "SuperAdmin").Wait();
                    }
                }
            }

        }
    }
}
