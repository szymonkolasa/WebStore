using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.Areas.Administration.ViewModels.Products;

namespace WebStore.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Owner,Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<ProductsController> _localizer;

        public ProductsController(
            ApplicationDbContext dbContext,
            IStringLocalizer<ProductsController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Administration/Products
        [HttpGet]
        public IActionResult Index()
        {
            var model = _dbContext.Products
                .Include(x => x.Category)
                .Where(x => !x.IsDeleted);

            return View(model);
        }

        //
        // GET: /Administration/Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = new List<string>();

            foreach (var category in _dbContext.Categories)
            {
                if (!_dbContext.Categories.Any(x => x.ParentCategory == category))
                {
                    categories.Add(category.Name);
                }
            }

            ViewData["Categories"] = categories;

            return View();
        }

        //
        // POST: /Administration/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            var categories = new List<string>();

            foreach (var category in _dbContext.Categories)
            {
                if (!_dbContext.Categories.Any(x => x.ParentCategory == category))
                {
                    categories.Add(category.Name);
                }
            }

            ViewData["Categories"] = categories;

            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            if (string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError(string.Empty, _localizer["Description field is required"]);
            }

            if (string.IsNullOrEmpty(model.ShortDescription))
            {
                ModelState.AddModelError(string.Empty, _localizer["Short description field is required"]);
            }

            if (model.Thumbnail != null && !imageTypes.Contains(model.Thumbnail.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (model.Images != null && model.Images.Any(x => !imageTypes.Contains(x.ContentType)))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    CreationDate = DateTime.Now,
                    Description = model.Description,
                    Name = model.Name,
                    Price = model.Price,
                    ShortDescription = model.ShortDescription,
                    IsDeleted = false
                };

                var category = _dbContext.Categories
                    .First(x => x.Name == model.Category);

                product.Category = category;
                product.Images = new List<ProductImage>();

                if (model.Images != null)
                {
                    foreach (var image in model.Images)
                    {
                        var img = new ProductImage();

                        using (var stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);
                            img.Image = stream.ToArray();
                            img.ImageType = image.ContentType;
                        }

                        product.Images.Add(img);
                    }
                }

                if (model.Thumbnail != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.Thumbnail.CopyToAsync(stream);
                        product.Image = stream.ToArray();
                        product.ImageType = model.Thumbnail.ContentType;
                    }
                }

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(ProductsController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Products/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var categories = new List<string>();

            foreach (var category in _dbContext.Categories)
            {
                if (!_dbContext.Categories.Any(x => x.ParentCategory == category))
                {
                    categories.Add(category.Name);
                }
            }

            ViewData["Categories"] = categories;

            var product = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .First(x => x.Id == id);
            
            var model = new ProductViewModel
            {
                Id = product.Id,
                Category = product.Category.Name,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                ProductImages = product.Images
            };

            return View(model);
        }

        //
        // POST: /Administration/Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var categories = new List<string>();

            foreach (var category in _dbContext.Categories)
            {
                if (!_dbContext.Categories.Any(x => x.ParentCategory == category))
                {
                    categories.Add(category.Name);
                }
            }

            ViewData["Categories"] = categories;

            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            if (string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError(string.Empty, _localizer["Description field is required"]);
            }

            if (string.IsNullOrEmpty(model.ShortDescription))
            {
                ModelState.AddModelError(string.Empty, _localizer["Short description field is required"]);
            }

            if (model.Images != null && model.Images.Any(x => !imageTypes.Contains(x.ContentType)))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (model.Thumbnail != null && !imageTypes.Contains(model.Thumbnail.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (ModelState.IsValid)
            {
                var product = _dbContext.Products
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .First(x => x.Id == model.Id);

                var category = _dbContext.Categories
                    .First(x => x.Name == model.Category);

                product.Category = category;
                product.Description = model.Description;
                product.Name = model.Name;
                product.Price = model.Price;
                product.ShortDescription = model.ShortDescription;

                if (product.Images is null)
                {
                    product.Images = new List<ProductImage>();
                }

                if (model.Images != null)
                {
                    foreach (var image in model.Images)
                    {
                        var img = new ProductImage();

                        using (var stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);
                            img.Image = stream.ToArray();
                        }

                        img.ImageType = image.ContentType;

                        product.Images.Add(img);
                    }
                }

                if (model.Thumbnail != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.Thumbnail.CopyToAsync(stream);
                        product.Image = stream.ToArray();
                        product.ImageType = model.Thumbnail.ContentType;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(ProductsController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Products/Details
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .First(x => x.Id == id);

            var model = new ProductViewModel
            {
                Category = product.Category.Name,
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ProductImages = product.Images,
                ShortDescription = product.ShortDescription
            };

            return View(model);
        }

        //
        // GET: /Administration/Products/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = _dbContext.Products
                .Include(x => x.Images)
                .First(x => x.Id == id);

            if (_dbContext.OrderItems.Any(x => x.ProductId == product.Id))
            {
                product.IsDeleted = true;
            }
            else
            {
                _dbContext.ProductImages.RemoveRange(product.Images);
                _dbContext.Products.Remove(product);
            }
            
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(ProductsController.Index));
        }

        #region Helpers

        //
        // GET: /Administration/Products/ProductImage
        [HttpGet]
        public async Task<IActionResult> ProductImage(Guid id)
        {
            var image = _dbContext.ProductImages
                .First(x => x.Id == id);

            if (image.Image != null)
            {
                var stream = new MemoryStream(image.Image);
                return new FileStreamResult(stream, image.ImageType);
            }

            return null;
        }

        //
        // GET: /Administration/Products/Thumbnail
        [HttpGet]
        public async Task<IActionResult> Thumbnail(Guid id)
        {
            var product = _dbContext.Products
                .First(x => x.Id == id);

            if (product.Image != null)
            {
                var stream = new MemoryStream(product.Image);
                return new FileStreamResult(stream, product.ImageType);
            }

            return null;
        }

        //
        // GET: /Administration/Products/DeleteImage
        [HttpGet]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var image = _dbContext.ProductImages
                .First(x => x.Id == id);

            _dbContext.Remove(image);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}