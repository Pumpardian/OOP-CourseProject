using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ACS_Reception.Application
{
    public class Mediator : IMediator
    {
        private readonly Dictionary<Type, object> handlers = [];

        public Mediator(IServiceProvider serviceProvider)
        {
            Assembly assembly = typeof(Mediator).Assembly;

            var types = assembly.GetTypes().Where(t => t.IsClass && t.GetInterfaces().Any(i =>
                       i.IsGenericType &&
                       (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))));

            foreach (var type in types)
            {
                var typeInstance = ActivatorUtilities.CreateInstance(serviceProvider, type);

                var @interface = type.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))).First();

                var requestType = @interface.GetGenericArguments()[0];
                handlers.TryAdd(requestType, typeInstance);
            }
        }

        public async Task Send(IRequest request)
        {
            if (handlers.TryGetValue(request.GetType(), out var handler))
            {
                var handlerInterface = handler.GetType().GetInterfaces()
                                .FirstOrDefault(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) &&
                                i.GetGenericArguments()[0] == request.GetType());

                var handleMethod = handlerInterface!.GetMethod("Handle");
                await (Task)handleMethod!.Invoke(handler, [request, default(CancellationToken)])!;
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (handlers.TryGetValue(request.GetType(), out var handler))
            {
                var handlerInterface = handler.GetType().GetInterfaces()
                                .FirstOrDefault(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) &&
                                i.GetGenericArguments()[0] == request.GetType());

                var handleMethod = handlerInterface!.GetMethod("Handle");
                return await (Task<TResponse>)handleMethod!.Invoke(handler, [request, default(CancellationToken)])!;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}