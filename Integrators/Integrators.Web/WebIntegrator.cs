using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.AspNetCore.Builder;

namespace Integrators.Web;
public class WebIntegrator
{
    public static IWebHostBuilder IntegrateBuilder<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        return WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<TStartup>();
    }

    public static IWebHost IntegrateHost<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        var builder = WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<TStartup>();
        return builder.Build();
    }

    public static async Task RunWebAsync<TStartup>(string[] args)
        where TStartup : WebStartupBase, new()
    {
        var host = IntegrateHost<TStartup>(args);

        await host.RunAsync();
    }
}
