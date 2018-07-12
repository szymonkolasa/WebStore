using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.Areas.Administration.ViewModels.Employees;

namespace WebStore.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Owner,Administrator")]
    public class EmployeesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<EmployeesController> _localizer;

        public EmployeesController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IStringLocalizer<EmployeesController> localizer)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Administration/Employees
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _userManager.GetUsersInRoleAsync("Employee");

            if (await _userManager.IsInRoleAsync(user, "Owner"))
            {
                foreach (var item in await _userManager.GetUsersInRoleAsync("Administrator"))
                {
                    model.Add(item);
                }
            }

            return View(model);
        }

        //
        // GET: /Administration/Employees/Add
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = new List<string> { "Employee" };
            
            if (await _userManager.IsInRoleAsync(user, "Owner"))
            {
                roles.Add("Administrator");
            }

            ViewData["RoleList"] = roles;
            return View();
        }

        //
        // POST: /Administration/Employees/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var password = GeneratePassword();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction(nameof(EmployeesController.EmployeeSummary), new { id = user.Id, password });
                }

                AddErrors(result);
            }

            var current = await _userManager.GetUserAsync(User);
            var roles = new List<string> { "Employee" };

            if (await _userManager.IsInRoleAsync(current, "Owner"))
            {
                roles.Add("Administrator");
            }

            ViewData["RoleList"] = roles;

            return View(model);
        }

        //
        // POST: /Administration/Employee/Summary
        [HttpGet]
        public async Task<IActionResult> EmployeeSummary(Guid id, string password)
        {
            ViewData["EmployeePassword"] = password;
            
            var model = _dbContext.Users
                .First(x => x.Id == id);

            ViewData["Role"] = (await _userManager.GetRolesAsync(model)).First();

            return View(model);
        }

        //
        // GET: /Administration/Employee/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var current = await _userManager.GetUserAsync(User);
            var roles = new List<string> { "", "Employee" };

            if (await _userManager.IsInRoleAsync(current, "Owner"))
            {
                roles.Add("Administrator");
            }

            ViewData["RoleList"] = roles;

            var user = _dbContext.Users
                .First(x => x.Id == id);

            var role = (await _userManager.GetRolesAsync(user)).First();

            var model = new EmployeeViewModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = role
            };

            return View(model);
        }

        //
        // POST: /Administration/Employees/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users
                    .First(x => x.Id == model.Id);

                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Count > 0)
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                }

                if (!string.IsNullOrEmpty(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                if (model.Email.ToLower() != user.Email.ToLower())
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                    await _userManager.ChangeEmailAsync(user, model.Email, token);
                    await _userManager.UpdateNormalizedEmailAsync(user);
                    await _userManager.SetUserNameAsync(user, model.Email);
                }

                if (model.PhoneNumber != user.PhoneNumber)
                {
                    var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
                    await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, token);
                }

                return RedirectToAction(nameof(EmployeesController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Employee/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = _dbContext.Users
                .First(x => x.Id == id);

            _dbContext.Users.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(EmployeesController.Index));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string GeneratePassword()
        {
            var random = new Random();
            var password = string.Empty;
            bool hasLower, hasUpper, hasDigit, hasSpecial;

            var chars = new List<char>
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
                };

            var digits = new List<char>
                {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                };

            var special = new List<char>
                {
                    '!', '@', '#', '$', '%', '^', '&', '*'
                };

            do
            {
                password = string.Empty;

                for (int i = 0; i < 8; i++)
                {
                    var result = random.Next() % 40;

                    if (result < 10)
                    {
                        password += chars[random.Next() % chars.Count];
                    }
                    else if (result < 20)
                    {
                        password += char.ToUpper(chars[random.Next() % chars.Count]);
                    }
                    else if (result < 30)
                    {
                        password += digits[random.Next() % digits.Count];
                    }
                    else
                    {
                        password += special[random.Next() % special.Count];
                    }
                }

                var uppers = chars.Select(x => char.ToUpper(x));

                hasLower = password.Any(x => chars.Contains(x));
                hasDigit = password.Any(x => digits.Contains(x));
                hasSpecial = password.Any(x => chars.Contains(x));
                hasUpper = password.Any(x => uppers.Contains(x));

            } while (!hasLower || !hasUpper || !hasDigit || !hasSpecial);

            return password;
        }

        #endregion
    }
}