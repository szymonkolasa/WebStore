using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Data.Entities;

namespace WebStore.Data
{
    public static class DataSeeder
    {
        /// <summary>
        /// Adds first run data like roles and user with owner role
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="roleManager">Application role manager</param>
        /// <param name="userManager">Application user manager</param>
        /// <returns>Returns context to continue operations</returns>
        public static async Task<ApplicationDbContext> SeedAsync(this ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            var roles = new List<string>
            {
                "Owner",
                "Administrator",
                "Employee"
            };

            // Adds roles
            foreach (var role in roles)
            {
                if (!(await roleManager.RoleExistsAsync(role)))
                {
                    var identityRole = new ApplicationRole
                    {
                        Name = role
                    };

                    await roleManager.CreateAsync(identityRole);
                }
            }

            // Check if user with Owner role exists
            var ownerRole = roleManager.Roles
                .Where(x => x.Name == "Owner")
                .First();

            var ownerExists = context.UserRoles
                .Where(x => x.RoleId == ownerRole.Id)
                .Count() > 0;

            // Seeds owner if not exists
            if (!ownerExists)
            {
                var owner = new ApplicationUser
                {
                    Email = "admin@store.com",
                    UserName = "admin@store.com"
                };

                await userManager.CreateAsync(owner, "12Ed6fg#");

                var token = await userManager.GenerateEmailConfirmationTokenAsync(owner);
                await userManager.ConfirmEmailAsync(owner, token);
                await userManager.AddToRoleAsync(owner, "Owner");
            }

            return context;
        }
    }
}
