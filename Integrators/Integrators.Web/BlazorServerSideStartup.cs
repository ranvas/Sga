using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrators.Web
{
    public class BlazorServerSideStartup : WebStartupBase
    {
        public override void ConfigureBase(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifeTime, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        public override void ConfigureServicesBase(IServiceCollection services, IConfiguration configuration)
        {
            var provider = services.BuildServiceProvider();
            var env = provider.GetRequiredService<IHostEnvironment>();
            var contentRootPath = env.ContentRootPath;

            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

    }
}
