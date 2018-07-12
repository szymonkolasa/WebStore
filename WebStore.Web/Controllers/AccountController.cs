using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.ViewModels.Account;

namespace WebStore.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IStringLocalizer<AccountController> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _localizer = localizer;
        }

        #region Unauthorized access

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (_dbContext.Users.Any(x => x.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError(string.Empty, _localizer["The user already exists"]);
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    
                    return RedirectToLocal(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, _localizer["Wrong username or password"]);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        #endregion
        
        //
        // GET: /Account/Index
        [HttpGet]
        public async Task<IActionResult> Index(string message = null)
        {
            return View(model: message);
        }

        //
        // GET: /Account/Email
        [HttpGet]
        public async Task<IActionResult> Email()
        {
            return View();
        }

        //
        // POST: /Account/Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(EmailViewModel model)
        {
            if (model.CurrentEmail == model.NewEmail)
            {
                ModelState.AddModelError(string.Empty, _localizer["The new email must be different"]);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

                if (checkPassword)
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);

                    if (result.Succeeded)
                    {
                        await _userManager.UpdateNormalizedEmailAsync(user);
                        return RedirectToAction(nameof(AccountController.Index), new { message = _localizer["Email has been changed"] });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["The password does not match"]);
                }
            }

            return View(model);
        }

        //
        // GET: /Account/Password
        [HttpGet]
        public async Task<IActionResult> Password()
        {
            return View();
        }

        //
        // POST: /Account/Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(PasswordViewModel model)
        {
            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(string.Empty, _localizer["The new password must be different"]);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(AccountController.Index), new { message = _localizer["Password has been changed"] });
                }

                AddErrors(result);
            }

            return View(model);
        }

        //
        // GET: /Account/Shipping
        [HttpGet]
        public async Task<IActionResult> Shipping()
        {
            var userId = _userManager.GetUserId(User);
            var users = _dbContext.Users.Include(x => x.ShippingInformations);
            var user = users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

            return View(user.ShippingInformations);
        }

        //
        // GET: /Account/ShippingInformation
        [HttpGet]
        public async Task<IActionResult> ShippingInformation(string id = null)
        {
            var userId = _userManager.GetUserId(User);
            var users = _dbContext.Users.Include(x => x.ShippingInformations);
            var user = users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

            var model = new ShippingInformationViewModel();

            if (id != null)
            {
                var result = user.ShippingInformations.FirstOrDefault(x => x.Id == Guid.Parse(id));

                if (result != null)
                {
                    model.City = result.City;
                    model.Country = result.Country;
                    model.FirstName = result.FirstName;
                    model.FlatNumber = result.FlatNumber;
                    model.HouseNumber = result.HouseNumber;
                    model.Id = result.Id.ToString();
                    model.LastName = result.LastName;
                    model.Name = result.Name;
                    model.PhoneNumber = result.PhoneNumber;
                    model.Street = result.Street;
                    model.ZipCode = result.ZipCode;
                }
            }

            return View(model);
        }

        //
        // POST: /Account/ShippingInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShippingInformation(ShippingInformationViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var users = _dbContext.Users.Include(x => x.ShippingInformations);
            var user = users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

            if (user.ShippingInformations.Any(x => x.Name.ToLower().CompareTo(model.Name.ToLower()) == 0))
            {
                ModelState.AddModelError(string.Empty, _localizer["Name must be unique"]);
            }

            if (ModelState.IsValid)
            {
                var entity = model.Id == null ? new ShippingInformation() : user.ShippingInformations.FirstOrDefault(x => x.Id == Guid.Parse(model.Id));

                entity.City = model.City;
                entity.Country = model.Country;
                entity.FirstName = model.FirstName;
                entity.FlatNumber = model.FlatNumber;
                entity.HouseNumber = model.HouseNumber;
                entity.LastName = model.LastName;
                entity.Name = model.Name;
                entity.PhoneNumber = model.PhoneNumber;
                entity.Street = model.Street;
                entity.ZipCode = model.ZipCode;

                if (model.Id == null)
                    user.ShippingInformations.Add(entity);
                
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return RedirectToAction(nameof(AccountController.Index), new { message = _localizer["Shipping information has been saved"] });
                }
            }

            return View(model);
        }

        //
        // GET: /Account/DeleteShippingInformation
        [HttpGet]
        public async Task<IActionResult> DeleteShippingInformation(string id = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var users = _dbContext.Users.Include(x => x.ShippingInformations);

            var shipping = _dbContext.ShippingInformations.FirstOrDefault(x => x.Id == Guid.Parse(id));
            _dbContext.ShippingInformations.Remove(shipping);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(AccountController.Shipping));
        }

        //
        // GET: /Account/GetShippingInformations
        [HttpGet]
        public async Task<IActionResult> GetShippingInformations()
        {
            var id = Guid.Parse(_userManager.GetUserId(User));

            var user = _dbContext.Users
                .Include(x => x.ShippingInformations)
                .First(x => x.Id == id);

            var model = user.ShippingInformations
                .Select(x => x.Name);

            return Ok(model);
        }

        //
        // GET: /Account/GetShippingInformation
        [HttpGet]
        public async Task<IActionResult> GetShippingInformation(string id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));

            var user = _dbContext.Users
                .Include(x => x.ShippingInformations)
                .First(x => x.Id == userId);

            var model = user.ShippingInformations
                .First(x => x.Name == id);

            return Ok(model);
        }

        //
        // GET: /Account/CurrentOrders
        [HttpGet]
        public async Task<IActionResult> CurrentOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = _dbContext.Orders
                .Include(x => x.Items)
                .Include(x => x.Purchaser)
                .Where(x => x.Purchaser.Id == user.Id 
                    && x.Status == Data.Enums.OrderStatus.New || x.Status == Data.Enums.OrderStatus.Preparation)
                .OrderByDescending(x => x.CreationDate);

            return View("OrderList", orders);
        }

        //
        // GET: /Account/OrderHistory
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _dbContext.Orders
                .Include(x => x.Items)
                .Include(x => x.Purchaser)
                .Where(x => x.Purchaser.Id == user.Id 
                    && x.Status != Data.Enums.OrderStatus.Preparation && x.Status != Data.Enums.OrderStatus.New)
                .OrderByDescending(x => x.CreationDate);

            return View("OrderList", orders);
        }

        //
        // GET: /Account/OrderDetails
        [HttpGet]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var model = _dbContext.Orders
                .Include(x => x.Items)
                .Include(x => x.Purchaser)
                .First(x => x.Id == id);

            return View(model);
        }

        //
        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, _localizer[error.Description]);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}