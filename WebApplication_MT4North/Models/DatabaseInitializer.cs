using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication_MT4North.Models
{
    public class DatabaseInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("user@localhost").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "user@localhost";
                user.Email = "user@localhost";
                user.FirstName = "Us";
                user.LastName = "Er";

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "BasicUser").Wait();
                }
            }


            if (userManager.FindByEmailAsync("admin@localhost").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "admin@localhost";
                user.Email = "admin@localhost";
                user.FirstName = "Ad";
                user.LastName = "Min";

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "AdminUser").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("BasicUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "BasicUser";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("AdminUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "AdminUser";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }

    }
}
