using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Integrators.Dispatcher
{
    internal class HostDispatcherHelper
    {
        internal static async Task InvokeVoid(DispatcherInfo info, object? request, IServiceScope scope)
        {
            var manager = scope.ServiceProvider.GetService(info.InstanceType);
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(info.InstanceType));
            }
            if (info.AsAsync)
            {
                dynamic? task = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                if(task != null) 
                    await task;
            }
            else
            {
                var task = Task.Run(() =>
                {
                    _ = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                }).ConfigureAwait(false);
                await task;
            }
        }

        internal static async Task<object?> Invoke(DispatcherInfo info, object? request, IServiceScope scope)
        {
            var manager = scope.ServiceProvider.GetService(info.InstanceType);
            if(manager == null)
            {
                throw new ArgumentNullException(nameof(info.InstanceType));
            }
            object? response;
            if (info.AsAsync)
            {
                dynamic? task = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                response = task != null ? await task : null;
            }
            else
            {
                var task = Task.Run(() =>
                {
                    response = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                    return response;
                }).ConfigureAwait(false);
                response = await task;
            }
            return response;
        }

        internal static bool IsAsync(MethodInfo info)
        {
            if (info.IsDefined(typeof(AsyncStateMachineAttribute), false) || (info.ReturnType.IsGenericType && info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
            {
                return true;
            }
            return false;
        }
    }
}
