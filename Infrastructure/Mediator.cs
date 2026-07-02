using System.Collections.Concurrent;
using System.Reflection;
using Catalog.Shared.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure
{
    internal class Mediator : IMediator
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> SendMethodCache = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethodCache = new();

        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _serviceProvider.GetRequiredService(handlerType);

            var handleMethod = HandleMethodCache.GetOrAdd(handlerType, t =>
                t.GetMethod("Handle") ?? throw new InvalidOperationException($"Handle method not found on {t}"));

            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });
            if (result is Task<TResponse> task)
                return await task;

            throw new InvalidOperationException($"Handler for {request.GetType()} did not return Task<{typeof(TResponse).Name}>");
        }

        public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            var handleMethod = HandleMethodCache.GetOrAdd(handlerType, t =>
                t.GetMethod("Handle") ?? throw new InvalidOperationException($"Handle method not found on {t}"));

            foreach (var handler in handlers)
            {
                var result = handleMethod.Invoke(handler, new object[] { notification, cancellationToken });
                if (result is Task task)
                    await task;
            }
        }
    }
}
