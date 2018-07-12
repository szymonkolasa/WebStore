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

namespace WebStore.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Employee, Owner, Administrator")]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<OrdersController> _localizer;

        public OrdersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IStringLocalizer<OrdersController> localizer)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _localizer = localizer;
        }


        //
        // GET: /Administration/Orders/Index
        [HttpGet]
        public async Task<IActionResult> Index(string filter = "New")
        {
            var orders = _dbContext.Orders
                .Include(x => x.Employee)
                .Include(x => x.Purchaser)
                .AsQueryable();

            if (filter == "New")
            {
                orders = orders.Where(x => x.Status == Data.Enums.OrderStatus.New && x.Employee == null);
            }

            if (filter == "My")
            {
                var user = await _userManager.GetUserAsync(User);
                orders = orders.Where(x => x.Employee == user);
            }

            orders = orders.OrderByDescending(x => x.CreationDate);

            ViewData["Filter"] = filter;
            return View(orders);
        }

        //
        // GET: /Administration/Orders/Assign
        [HttpGet]
        public async Task<IActionResult> Assign(Guid id)
        {
            var order = _dbContext.Orders
                .First(x => x.Id == id);

            var user = await _userManager.GetUserAsync(User);
            order.Employee = user;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(OrdersController.Index), new { filter = "My" });
        }

        //
        // GET: /Administration/Orders/Detail
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var order = _dbContext.Orders
                .Include(x => x.Employee)
                .Include(x => x.Purchaser)
                .Include(x => x.Items)
                .First(x => x.Id == id);
            
            return View(order);
        }

        //
        // GET: /Administration/Orders/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var order = _dbContext.Orders
                .Include(x => x.Employee)
                .Include(x => x.Purchaser)
                .Include(x => x.Items)
                .First(x => x.Id == id);

            ViewData["OrderStatus"] = (int)order.Status;

            return View(order);
        }

        //
        // POST: /Administration/Orders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, int status)
        {
            var order = _dbContext.Orders
                .First(x => x.Id == id);

            ViewData["OrderStatus"] = status;
            order.Status = (Data.Enums.OrderStatus)status;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(OrdersController.Edit), new { id });
        }
    }
}