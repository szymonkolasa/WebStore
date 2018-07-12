using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebStore.Data;

namespace WebStore.Web.Controllers
{
    public class BannerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStringLocalizer<BannerController> _localizer;

        public BannerController(
            ApplicationDbContext dbContext,
            IStringLocalizer<BannerController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        //
        // GET: /Banner/Index
        public IActionResult Index(Guid id)
        {
            var banner = _dbContext.Banners
                .First(x => x.Id == id);

            return LocalRedirect(banner.Link);
        }

        //
        // GET: /Banner/GetImage
        [HttpGet]
        public async Task<IActionResult> GetImage(Guid id)
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
    }
}