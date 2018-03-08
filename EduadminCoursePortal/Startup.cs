using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.BLL;
using StackExchange.Redis;

namespace EduadminCoursePortal
{
    public static class LocalizerProvider
    {
        public static IStringLocalizer<SharedResources> Localizer { get; set; }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            LocalizerProvider.Localizer = services.BuildServiceProvider().GetService<IStringLocalizer<SharedResources>>();
            services.AddSingleton<IMemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));
            services.AddMemoryCache();
            services.AddTransient<BLL.ICourseTemplateService, BLL.CourseTemplateService>();
            services.AddTransient<IToken, Token>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider =
                            (type, factory) => factory.Create(typeof(SharedResources));
                    });
            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("sv-SE")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("sv-SE"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=CourseTemplate}/{action=CourseTemplatesList}/{id?}");
            });
        }
    }
}
