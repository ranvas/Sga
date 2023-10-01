using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Integrators.Core
{
    public class 合気道Integrator
    {
        public IConfiguration? Configuration { get; set; }

        public async Task RunDefaultIntegrator(string[] args)
        {
            await RunAsync(CreateHostBuilder(args), new CancellationToken());
        }

        private IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Init(services, hostContext.Configuration);
                });

        private async Task RunAsync(IHostBuilder taskToWatch, CancellationToken token = default)
        {
            try
            {
                await taskToWatch
                    .Build()
                    .RunAsync(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected virtual void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}