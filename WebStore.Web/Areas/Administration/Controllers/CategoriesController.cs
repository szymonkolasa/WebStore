using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.Areas.Administration.ViewModels.Categories;

namespace WebStore.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Owner,Administrator")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<CategoriesController> _localizer;

        public CategoriesController(
            ApplicationDbContext dbContext,
            IStringLocalizer<CategoriesController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Administration/Categories/Index
        [HttpGet]
        public async Task<IActionResult> Index(int page = 0)
        {
            var term = ViewData["SearchTerm"] as string;

            var data = _dbContext.Categories.AsQueryable();

            if (term != null && !string.IsNullOrEmpty(term.Trim()))
            {
                data = data.Where(x => x.Name.ToLower().Contains(term.ToLower()));
            }

            int pageSize = 20;
            var model = data.Skip(page * pageSize)
                .Take(pageSize)
                .Select(x => new CategoriesViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentCategory = x.ParentCategory,
                    ShowOnBar = x.ShowOnBar
                });

            return View(model);
        }

        //
        // GET: /Administration/Categories/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["ParentCategories"] = _dbContext.Categories.Select(x => x.Name);
            return View();
        }

        //
        // POST: /Administration/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            ViewData["ParentCategories"] = _dbContext.Categories.Select(x => x.Name);

            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            if (_dbContext.Categories.Any(x => x.Name.ToLower().CompareTo(model.Name.ToLower()) == 0))
            {
                ModelState.AddModelError(string.Empty, _localizer["Category already exists"]);
            }

            if (model.CategoryImage != null && !imageTypes.Contains(model.CategoryImage.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    ShowOnBar = model.ShowOnBar
                };

                if (model.CategoryImage != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.CategoryImage.CopyToAsync(stream);
                        category.Image = stream.ToArray();
                    }

                    category.ImageType = model.CategoryImage.ContentType;
                }

                if (!string.IsNullOrEmpty(model.ParentCategory))
                {
                    category.ParentCategory = _dbContext.Categories.FirstOrDefault(x => x.Name == model.ParentCategory);
                }

                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(CategoriesController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Categories/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = _dbContext.Categories
                .Include(x => x.ParentCategory)
                .First(x => x.Id == id);

            ViewData["ParentCategories"] = _dbContext.Categories
                .Where(x => x.Id != id)
                .Select(x => x.Name);

            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ShowOnBar = category.ShowOnBar
            };

            model.ParentCategory = category.ParentCategory != null ? category.ParentCategory.Name : string.Empty;

            return View(model);
        }

        //
        // POST: /Administration/Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            ViewData["ParentCategories"] = _dbContext.Categories
                .Where(x => x.Id != model.Id)
                .Select(x => x.Name);

            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            var category = _dbContext.Categories
                .First(x => x.Id == model.Id);

            if (category.Name.ToLower().CompareTo(model.Name.ToLower()) != 0 && _dbContext.Categories.Any(x => x.Name.ToLower().CompareTo(model.Name.ToLower()) == 0))
            {
                ModelState.AddModelError(string.Empty, _localizer["Category already exists"]);
            }

            if (model.CategoryImage != null && !imageTypes.Contains(model.CategoryImage.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (ModelState.IsValid)
            {
                category.Name = model.Name;
                category.ShowOnBar = model.ShowOnBar;

                if (model.CategoryImage != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.CategoryImage.CopyToAsync(stream);
                        category.Image = stream.ToArray();
                    }

                    category.ImageType = model.CategoryImage.ContentType;
                }

                if (!string.IsNullOrEmpty(model.ParentCategory))
                {
                    category.ParentCategory = _dbContext.Categories
                        .FirstOrDefault(x => x.Name == model.ParentCategory);
                }
                else
                {
                    category.ParentCategory = null;
                }

                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(CategoriesController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Categories/Details
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var category = _dbContext.Categories
                .Include(x => x.ParentCategory)
                .First(x => x.Id == id);

            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ShowOnBar = category.ShowOnBar
            };

            model.ParentCategory = category.ParentCategory != null ? category.ParentCategory.Name : string.Empty;

            return View(model);
        }

        //
        // GET: /Administration/Categories/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            DeleteCategories(id);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(CategoriesController.Index));
        }

        #region Helpers

        //
        // GET: /Administration/Categories/CategoryImage
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

        /// <summary>
        /// Removes category with all subcategories
        /// </summary>
        /// <param name="id">Category id</param>
        private async void DeleteCategories(Guid id)
        {
            var category = _dbContext.Categories
                .First(x => x.Id == id);

            var subCategories = _dbContext.Categories
                .Include(x => x.ParentCategory)
                .Where(x => x.ParentCategory == category);

            if (subCategories.Count() > 0)
            {
                foreach (var sub in subCategories)
                {
                    DeleteCategories(sub.Id);
                }
            }

            _dbContext.Categories.Remove(category);
        }

        #endregion
    }
}