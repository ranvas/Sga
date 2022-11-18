using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.AspNetCore.Builder;
using Integrators.Core;

namespace Integrators.Web;
public class WebIntegrator : 合気道Integrator
{
    public IWebHostBuilder CreateAdvancedWebHostBuilder<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        return WebHost
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                Init(services, hostContext.Configuration);
            })
            .UseStartup<TStartup>();
    }

    public IHostBuilder CreateDefaultWebHostBuilder<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((hostContext, services) => 
                { 
                    Init(services, hostContext.Configuration); 
                });
                webBuilder.UseStartup<TStartup>();
            });
    }

    public async Task RunWebAsync<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        var host = CreateDefaultWebHostBuilder<TStartup>(args);
        await RunAsync(host, new CancellationToken());
    }
}
