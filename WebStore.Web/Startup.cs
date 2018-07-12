using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebStore.Data;
using WebStore.Data.Entities;

namespace WebStore.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.SlidingExpiration = true;
                });

            // add localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // add MVC with localization
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
                .AddDataAnnotationsLocalization();

            // Add Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var database = services.BuildServiceProvider()
                .GetService<ApplicationDbContext>();

            var userManager = services.BuildServiceProvider()
                .GetService<UserManager<ApplicationUser>>();

            var roleManager = services.BuildServiceProvider()
                .GetService<RoleManager<ApplicationRole>>();

            // Migrates database
            database.Database
                .Migrate();

            database.SeedAsync(roleManager, userManager)
                .Wait();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Add localization cultures
            var defaultCulture = "pl-PL";
            var cultures = new[]
            {
                new CultureInfo(defaultCulture),
                new CultureInfo("en-US")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = cultures,
                SupportedUICultures = cultures
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "admin",
                    areaName: "Administration",
                    template: "Administration/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
