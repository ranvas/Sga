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
    public class 合気道Startup : WebStartupBase
    {
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifeTime, IConfiguration configuration)
        {
            
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            
        }
    }
}
