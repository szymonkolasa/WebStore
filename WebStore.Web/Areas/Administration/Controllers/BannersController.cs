using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebStore.Data;
using WebStore.Data.Entities;
using WebStore.Web.Areas.Administration.ViewModels.Banners;

namespace WebStore.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Employee, Owner,Administrator")]
    public class BannersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<BannersController> _localizer;

        public BannersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IStringLocalizer<BannersController> localizer)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Administration/Banners
        [HttpGet]
        public IActionResult Index()
        {
            var model = _dbContext.Banners;

            return View(model);
        }

        //
        // GET: /Administration/Banners/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        //
        // POST: /Administration/Banners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerViewModel model)
        {
            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            if (model.File != null && !imageTypes.Contains(model.File.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError(string.Empty, _localizer["The Start date must be earlier than End date"]);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var banner = new Banner
                {
                    Creator = user,
                    EndDate = model.EndDate,
                    Link = model.Link,
                    Name = model.Name,
                    StartDate = model.StartDate
                };

                if (model.File != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.File.CopyToAsync(stream);
                        banner.Image = stream.ToArray();
                        banner.ImageType = model.File.ContentType;
                    }
                }

                _dbContext.Banners.Add(banner);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(BannersController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Banners/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var banner = _dbContext.Banners
                .First(x => x.Id == id);

            var model = new BannerViewModel
            {
                Id = banner.Id,
                EndDate = banner.EndDate,
                Link = banner.Link,
                Name =  banner.Name,
                StartDate = banner.StartDate
            };

            return View(model);
        }

        //
        // POST: /Administration/banners/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BannerViewModel model)
        {
            var imageTypes = new string[]
            {
                "image/jpeg",
                "image/png"
            };

            if (model.File != null && !imageTypes.Contains(model.File.ContentType))
            {
                ModelState.AddModelError(string.Empty, _localizer["Wrong image type"]);
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError(string.Empty, _localizer["The Start date must be earlier than End date"]);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var banner = _dbContext.Banners
                    .First(x => x.Id == model.Id);

                banner.EndDate = model.EndDate;
                banner.Link = model.Link;
                banner.Name = model.Name;
                banner.StartDate = model.StartDate;
                banner.Creator = user;

                if (model.File != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.File.CopyToAsync(stream);
                        banner.Image = stream.ToArray();
                        banner.ImageType = model.File.ContentType;
                    }
                }

                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(BannersController.Index));
            }

            return View(model);
        }

        //
        // GET: /Administration/Banners/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var banner = _dbContext.Banners
                .First(x => x.Id == id);

            _dbContext.Banners.Remove(banner);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(BannersController.Index));
        }

        #region Helpers

        //
        // GET: /Administration/Banners/BannerImage
        [HttpGet]
        public async Task<IActionResult> BannerImage(Guid id)
        {
            var banner = _dbContext.Banners
                .First(x => x.Id == id);

            if (banner.Image != null)
            {
                var stream = new MemoryStream(banner.Image);
                return new FileStreamResult(stream, banner.ImageType);
            }

            return null;
        }

        #endregion
    }
}