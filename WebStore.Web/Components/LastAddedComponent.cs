using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebStore.Data;

namespace WebStore.Web.Components
{
    public class LastAddedComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public LastAddedComponent(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = _dbContext.Products
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreationDate)
                .Take(20);

            return View("_LastAddedPartial", model);
        }
    }
}
