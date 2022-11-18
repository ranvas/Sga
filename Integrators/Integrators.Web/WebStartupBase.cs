using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    public abstract class WebStartupBase 
    {
        public abstract void ConfigureServicesBase(IServiceCollection services, IConfiguration configuration);

        public abstract void ConfigureBase(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifeTime, IConfiguration configuration);

        public void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var appLifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            ConfigureBase(app, env, appLifeTime, configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            ConfigureServicesBase(services, configuration);
            services.BuildServiceProvider();
        }
    }
}
