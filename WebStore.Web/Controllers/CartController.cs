using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.Extensions;
using WebStore.Web.Models.Cart;
using WebStore.Web.ViewModels.Cart;

namespace WebStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<CartController> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private HttpContext _httpContext => _httpContextAccessor.HttpContext;

        private Cart Cart
        {
            get
            {
                Cart cart = _httpContext.Session.Get<Cart>("Cart");

                if (cart is null)
                {
                    cart = new Cart();
                    _httpContext.Session.Set("Cart", cart);
                }

                return cart;
            }
            set
            {
                _httpContext.Session.Set("Cart", value);
            }
        }

        public CartController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IStringLocalizer<CartController> localizer,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        //
        // GET: /Cart/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(Cart.Items);
        }

        //
        // GET: /Cart/Count
        [HttpGet]
        public async Task<IActionResult> Count()
        {
            return Ok(Cart.Count > 100 ? "99+" : Cart.Count.ToString());
        }

        //
        // GET: /Cart/Add
        [HttpGet]
        public async Task<IActionResult> Add(Guid id)
        {
            var product = _dbContext.Products
                .First(x => x.Id == id);

            var cart = Cart;
            cart.Add(product);
            Cart = cart;

            return RedirectToAction(nameof(CartController.Index));
        }

        //
        // GET: /Cart/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = _dbContext.Products
                .First(x => x.Id == id);

            var cart = Cart;
            cart.Remove(product);
            Cart = cart;

            return RedirectToAction(nameof(CartController.Index));
        }

        //
        // GET: /Cart/Plus
        [HttpGet]
        public async Task<IActionResult> Plus(Guid id)
        {
            var product = _dbContext.Products
                .First(x => x.Id == id);

            var cart = Cart;
            cart.Add(product);
            Cart = cart;

            return RedirectToAction(nameof(CartController.Index));
        }

        //
        // GET: /Cart/Minus
        [HttpGet]
        public async Task<IActionResult> Minus(Guid id)
        {
            var product = _dbContext.Products
                .First(x => x.Id == id);

            var cart = Cart;
            cart.Add(product, -1);
            Cart = cart;

            return RedirectToAction(nameof(CartController.Index));
        }

        //
        // GET: /Cart/Total
        [HttpGet]
        public async Task<IActionResult> Total()
        {
            return Ok(Cart.Total.ToString("C"));
        }

        //
        // GET: /Cart/Order
        [HttpGet]
        public async Task<IActionResult> Order()
        {
            return View();
        }

        //
        //POST: /Cart/Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var name = DateTime.Now.ToString("yyyy/MM/dd/hhmmss");

                var order = new Order
                {
                    City = model.City,
                    Country = model.Country,
                    CreationDate = DateTime.Now,
                    FirstName = model.FirstName,
                    FlatNumber = model.FlatNumber,
                    HouseNumber = model.HouseNumber,
                    IsGift = model.IsGift,
                    LastName = model.LastName,
                    Name = name,
                    PhoneNumber = model.PhoneNumber,
                    Status = Data.Enums.OrderStatus.New,
                    Street = model.Street,
                    ZipCode = model.ZipCode
                };

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    order.Purchaser = user;
                }

                order.Items = new List<OrderItem>();

                var cart = Cart;

                foreach (var item in Cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        Price = item.Product.Price,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity
                    };

                    order.Items.Add(orderItem);
                }

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                cart.Clear();

                Cart = cart;

                return RedirectToAction(nameof(CartController.OrderSummary), new { id = order.Id });
            }

            return View(model);
        }

        //
        // GET: /Cart/OrderSummary
        [HttpGet]
        public async Task<IActionResult> OrderSummary(Guid id)
        {
            var model = _dbContext.Orders
                .Include(x => x.Items)
                .Include(x => x.Purchaser)
                .First(x => x.Id == id);

            return View(model);
        }
    }
}