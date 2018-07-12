using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebStore.Data;

namespace WebStore.Web.Components
{
    public class BannerComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public BannerComponent(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = _dbContext.Banners
                .Where(x => DateTime.Now > x.StartDate && DateTime.Now < x.EndDate);

            return View("_BannerPartial", model);
        }
    }
}
