using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrators.Abstractions
{
    public interface IIntegrator
    {
        Task RunDefaultIntegrator(string[] args);
        Task RunDefaultIntegrator(string[] args, Action<IServiceCollection, IConfiguration> init);
    }
}
