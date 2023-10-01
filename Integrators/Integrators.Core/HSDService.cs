using Integrators.Abstractions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrators.Core
{
    public class HSDService : IHostedService
    {
        private readonly IDispatcher _dispatcher;
        private readonly string _keyStart;
        private readonly string _keyStop;

        public HSDService(IDispatcher dispatcher, string keyStart, string keyStop)
        {
            _dispatcher = dispatcher;
            _keyStart = keyStart;
            _keyStop = keyStop;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Работа HSDService началась");
            if (string.IsNullOrEmpty(_keyStart))
                return;
            await _dispatcher.DispatchSimple(_keyStart);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_keyStop))
                return;
            Console.WriteLine("Работа HSDService завершена");
            await _dispatcher.DispatchSimple(_keyStop);
        }

    }
}
