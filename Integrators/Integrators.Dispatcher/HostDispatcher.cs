using Integrators.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Integrators.Dispatcher
{
    public class HostDispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private ConcurrentDictionary<string, DispatcherInfo> RegisteredMethods { get; } = new();
        public HostDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual IDispatcher RegisterService<TIn, TInstance>(string methodName) =>
            RegisterService<TInstance>(typeof(TIn).Name, typeof(TIn), methodName);

        /// <summary>
        /// Register TInstance for using string key and string methodName of reflection
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="key"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public virtual IDispatcher RegisterService<TInstance>(string key, string methodName) =>
            RegisterService<TInstance>(key, null, methodName);

        /// <summary>
        /// Register TInstance for using string key and string methodName of reflection and receive Type typeRequest
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="key"></param>
        /// <param name="typeRequest"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public virtual IDispatcher RegisterService<TInstance>(string key, Type? typeRequest, string methodName)
        {
            if (!TryRegisterMethod<TInstance>(key, typeRequest, methodName))
            {
                throw new ApplicationException($"registering key:{key} for {methodName} failed");
            }
            return this;
        }

        public virtual async Task<TOut?> DispatchSimple<TIn, TOut>(TIn request) =>
            await DispatchSimple<TOut>(request, typeof(TIn).Name);

        public virtual async Task<TOut?> DispatchSimple<TOut>(string key) =>
            await DispatchSimple<TOut>(null, key);

        public virtual async Task<TOut?> DispatchSimple<TIn, TOut>(string key, TIn request) =>
            await DispatchSimple<TOut>(request, key);

        private async Task<TOut?> DispatchSimple<TOut>(object? request, string key)
        {
            var info = GetMethod(key);
            if (info == null)
                throw new NotImplementedException($"The handler for key {key} is not registered");
            if (!ReturnTypeIsAssignableTo<TOut>(info))
                throw new InvalidCastException($"The handler for key {key} returns {info.ResponseType}, although {typeof(TOut)} is expected");
            using var scope = _serviceProvider.CreateScope();
            return (TOut?)await HostDispatcherHelper.Invoke(info, request, scope);
        }

        private DispatcherInfo? GetMethod(string key)
        {
            if (RegisteredMethods.TryGetValue(key, out DispatcherInfo? info) && info.Resolved)
                return info;
            return default;
        }

        private bool ReturnTypeIsAssignableTo<T>(DispatcherInfo info)
        {
            return info.ResponseType == typeof(T) || (info.ResponseType?.IsAssignableTo(typeof(T)) ?? false);
        }

        private bool TryRegisterMethod<TIn, TInstance>(string methodName) =>
            TryRegisterMethod<TInstance>(typeof(TIn), methodName);

        private bool TryRegisterMethod<TInstance>(Type typeRequest, string methodName) =>
            TryRegisterMethod<TInstance>(typeRequest.Name, typeRequest, methodName);

        private bool TryRegisterMethod<TInstance>(string key, string methodName) =>
            TryRegisterMethod<TInstance>(key, null, methodName);

        private bool TryRegisterMethod<TInstance>(string key, Type? typeRequest, string methodName)
        {
            var typeInstance = typeof(TInstance);
            if (RegisteredMethods.ContainsKey(key))
                return false;
            var info = new DispatcherInfo(key, typeInstance, methodName)
            {
                RequestType = typeRequest
            };
            MethodInfo? methodInfo;
            try
            {
                methodInfo = info.InstanceType.GetMethod(info.MethodName, typeRequest != null ? new[] { typeRequest } : Array.Empty<Type>());
            }
            catch
            {
                return false;
            }

            if (methodInfo == null)
            {
                RegisteredMethods.AddOrUpdate(key, info, (_, _) => info);
                return false;
            }
            var asAsync = HostDispatcherHelper.IsAsync(methodInfo);
            info.ResponseType = asAsync ? methodInfo.ReturnType.GenericTypeArguments[0] : methodInfo.ReturnType;
            info.AsAsync = asAsync;
            info.MethodInfo = methodInfo;
            info.Resolved = true;
            RegisteredMethods.AddOrUpdate(key, info, (_, _) => info);
            return true;
        }
    }
}