using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebStore.Data;

namespace WebStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(
            ApplicationDbContext dbContext,
            IStringLocalizer<HomeController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Home
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}