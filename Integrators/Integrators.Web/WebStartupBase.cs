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
    public abstract class WebStartupBase : IStartup
    {
        public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifeTime, IConfiguration configuration);

        public void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var appLifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            Configure(app, env, appLifeTime, configuration);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            ConfigureServices(services, configuration);
            return services.BuildServiceProvider();
        }
    }
}
