using Microsoft.AspNetCore.Mvc;
using WebStore.Data;

namespace WebStore.Web.Components
{
    public class MenuComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public MenuComponent(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = _dbContext.Categories;
            return View("_MenuPartial", model);
        }
    }
}
