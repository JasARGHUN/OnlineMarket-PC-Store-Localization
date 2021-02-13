using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineMarket.Utility;
using ReflectionIT.Mvc.Paging;
using Stripe;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OnlineMarket.DataAccess.Data.Initializer;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;

namespace OnlineMarket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // These email processing services handle and send a confirmation email. To change go to OnlineMarket.Utility/MailJetSettings and MailJetEmailSender.
            // For API's change go to OnlineMarket/appsettings.json
            services.AddTransient<IEmailSender, MailJetEmailSender>();

            // BrainTree services
            // For API's keys change go to OnlineMarket/appsettings.json
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

            // Twilio services
            // For API's id and key change go to OnlineMarket/appsettings.json
            services.Configure<SmsSenderSettings>(Configuration.GetSection("Twilio"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddScoped<IDbInitializer, DbInitializer>(); // Initialize administrator and roles to database
            services.AddScoped<IInfoRepository, InfoRepository>();
            services.AddScoped<IAppDataRepository, AppDataRepository>();
            services.AddScoped<IAppAddressRepository, AppAddressRepository>();
            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            // Access level handler service in Identity, views located on Areas/Identity/Pages/Account
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // Setting and Configuration make in this area OnlineMarket/Areas/Identity/Pages/Account/ExternalLogin.cshtml and ExternalLogin.cshtml.cs
            services.AddAuthentication().AddFacebook(options =>
            {
                // This is where you change the ID and Secret to your Facebook API ID and secret.
                options.AppId = "383519965985250";
                options.AppSecret = "dfabdcd017cfd140220b2838d06130f3";
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                // This is where you change the ID and Secret to your Google(OAuth) API ID and secret.
                options.ClientId = "448260474134-o84vqln8fnajfhvdj1tdqosrhq2k2f50.apps.googleusercontent.com";
                options.ClientSecret = "TQb2p3eiiiBBx_Y77k0IDMZd";
            });

            services.AddMemoryCache();

            //Localization and Globalization
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru"),
                    new CultureInfo("kk-kz")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(50);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddPaging(options => {
                options.ViewName = "Bootstrap4";
                options.HtmlIndicatorDown = " <span>&darr;</span>";
                options.HtmlIndicatorUp = " <span>&uarr;</span>";
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            dbInitializer.Initialize();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: null,
                   pattern: "{category}/Page{productPage:int}",
                   defaults: new { area = "Customer", controller = "Home", action = "Index" }
                   );
                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "Page{productPage:int}",
                    defaults: new { area = "Customer", controller = "Home", action = "Index", productPage = 1 }
                    );
                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{category}",
                    defaults: new { area = "Customer", controller = "Home", action = "Index", productPage = 1 }
                    );
                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "",
                    defaults: new { area = "Customer", controller = "Home", action = "Index", productPage = 1 }
                    );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

        }
    }
}
