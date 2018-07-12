using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Web.ViewModels.Category;

namespace WebStore.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<CategoryController> _localizer;

        public CategoryController(
            ApplicationDbContext dbContext,
            IStringLocalizer<CategoryController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // /Category
        [HttpGet]
        public IActionResult Index(Guid id)
        {
            var parent = _dbContext.Categories
                .Include(x => x.ParentCategory)
                .First(x => x.Id == id);

            var categories = _dbContext.Categories
                .Include(x => x.ParentCategory)
                .Where(x => x.ParentCategory.Id == id);

            if (categories.Count() == 0)
            {
                return RedirectToAction(nameof(ProductController.Index), "Product", new { id });
            }

            var model = new DisplayCategoryViewModel
            {
                ParentCategory = parent,
                Categories = categories
            };

            return View(model);
        }

        //
        // GET: /Category/All
        [HttpGet]
        public async Task<IActionResult> All(Guid id)
        {
            return RedirectToAction(nameof(ProductController.Index), "Product", new { id });
        }

        #region Helpers

        //
        // GET: /Category/CategoryImage
        [HttpGet]
        public async Task<IActionResult> CategoryImage(Guid id)
        {
            var category = _dbContext.Categories
                .First(x => x.Id == id);

            if (category.Image != null)
            {
                var stream = new MemoryStream(category.Image);
                return new FileStreamResult(stream, category.ImageType);
            }

            return null;
        }

        #endregion
    }
}