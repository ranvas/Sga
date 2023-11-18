using Integrators.Abstractions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrators.Core
{
    public class DispatchedHostedService : IHostedService
    {
        public const string StartKey = "start_hosted_service";
        public const string StopKey = "stop_hosted_service";

        IDispatcher _dispatcher;
        public DispatchedHostedService(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_dispatcher.ContainsKey(StartKey))
            {
                Console.WriteLine(StartKey);
                _dispatcher.DispatchSimple(StartKey);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_dispatcher.ContainsKey(StopKey))
            {
                Console.WriteLine(StopKey);
                _dispatcher.DispatchSimple(StopKey);
            }
            return Task.CompletedTask;
        }
    }
}
