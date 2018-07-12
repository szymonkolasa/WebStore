using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.ViewModels.Product;

namespace WebStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<ProductController> _localizer;

        public ProductController(
            ApplicationDbContext dbContext,
            IStringLocalizer<ProductController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Products/
        [HttpGet]
        public IActionResult Index(Guid id, int page = 1, string filter = null)
        {
            var pageSize = 6;

            var category = _dbContext.Categories
                .First(x => x.Id == id);

            var products = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Where(x => x.Category.Id == id)
                .ToList();

            if (products is null || products.Count == 0)
            {
                GetCategoryProduct(products, id);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();

                if (filter == "p-asc")
                {
                    products = products.OrderBy(x => x.Price)
                        .ToList();
                }

                if (filter == "p-desc")
                {
                    products = products.OrderByDescending(x => x.Price)
                        .ToList();
                }

                if (filter == "n-asc")
                {
                    products = products.OrderBy(x => x.Name)
                        .ToList();
                }

                if (filter == "n-desc")
                {
                    products = products.OrderByDescending(x => x.Name)
                        .ToList();
                }
            }
            else
            {
                products = products.OrderByDescending(x => x.CreationDate)
                    .ToList();
            }

            var pages = products.Count / pageSize + (products.Count % pageSize > 0 ? 1 : 0);

            products = products.Where(x => !x.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Filter"] = filter;

            var model = new ProductViewModel
            {
                Category = category,
                Page = page,
                Pages = pages,
                Products = products
            };

            return View(model);
        }

        //
        // GET: /Products/Detail
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = _dbContext.Products
                .Include(x => x.Images)
                .First(x => x.Id == id);

            return View(product);
        }

        #region Helpers

        //
        // GET: /Products/Thumbnail
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
        // GET: /Product/GetImage
        [HttpGet]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var image = _dbContext.ProductImages
                .First(x => x.Id == id);

            var stream = new MemoryStream(image.Image);
            return new FileStreamResult(stream, image.ImageType);
        }

        private void GetCategoryProduct(List<Product> model, Guid id)
        {
            var category = _dbContext.Categories
                .Include(x => x.Products)
                .First(x => x.Id == id);

            var subCategories = _dbContext.Categories
                .Include(x => x.Products)
                .Where(x => x.ParentCategory.Id == category.Id);

            if (subCategories.Count() > 0)
            {
                foreach (var sub in subCategories)
                {
                    GetCategoryProduct(model, sub.Id);
                }
            }

            model.AddRange(category.Products);
        }

        #endregion
    }
}