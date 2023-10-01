using Integrators.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBot.DataSphere;

namespace Integrators.Core
{
    /// <summary>
    /// HostedService to Dispatcher integrator
    /// </summary>
    public class HSDIntegrator<TDispatcher, TService>
        : 合気道Integrator
        where TDispatcher : IDispatcher
        where TService : class
    {
        Func<IServiceProvider, IDispatcher> _dispatcherFactory;
        Func<IServiceProvider, TService> _serviceFactory;
        string _keyStart;
        string _keyStop;

        public HSDIntegrator(Func<IServiceProvider, IDispatcher> dispatcherFactory, Func<IServiceProvider, TService> serviceFactory, string keyStart): this(dispatcherFactory, serviceFactory,  keyStart, string.Empty)
        {
        }
        public HSDIntegrator(Func<IServiceProvider, IDispatcher> dispatcherFactory, Func<IServiceProvider, TService> serviceFactory, string keyStart, string keyStop)
        {
            _dispatcherFactory = dispatcherFactory;
            _serviceFactory = serviceFactory;
            _keyStart = keyStart;
            _keyStop = keyStop;
        }

        protected override void Init(IServiceCollection services, IConfiguration configuration)
        {
            base.Init(services, configuration);
            services.AddSingleton(_serviceFactory);
            services.AddSingleton(_dispatcherFactory);
            services.AddHostedService(RegisterHSD);
        }

        private HSDService RegisterHSD(IServiceProvider provider)
        {
            var dispatcher = provider.GetRequiredService<IDispatcher>();

            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));
            return new HSDService(dispatcher, _keyStart, _keyStop);
        }
    }
}
