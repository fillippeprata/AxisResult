using System.Reflection;
using AxisTrix.Bus;
using AxisTrix.CQRS.Commands;
using AxisTrix.CQRS.Handlers;
using AxisTrix.CQRS.Queries;
using AxisTrix.DependencyInjection;
using AxisTrix.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.CQRS;

public static class DependencyInjection
{
    extension(ServiceCollectionBuilder builder)
    {
        internal void AddMediatorHandler() => builder.Services.AddScoped<IAxisMediatorHandler, AxisMediatorHandler>();

        public ServiceCollectionBuilder AddCqrsMediator(Assembly assembly)
        {
            builder.AddValidationAssemblies(assembly);

            RegisterHandlers(builder.Services, assembly, typeof(IAxisCommand<>));
            RegisterHandlers(builder.Services, assembly, typeof(IAxisCommandHandler<>));
            RegisterHandlers(builder.Services, assembly, typeof(IAxisCommandHandler<,>));
            RegisterHandlers(builder.Services, assembly, typeof(IAxisQueryHandler<,>));
            RegisterHandlers(builder.Services, assembly, typeof(IAxisEventHandler<>));
            return builder;
        }
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type handlerInterfaceType)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false, IsGenericType: false } &&
                type.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == handlerInterfaceType))
            .Distinct();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == handlerInterfaceType)
                .Distinct();

            foreach (var interfaceType in interfaces)
                services.AddTransient(interfaceType, handlerType);
        }
    }
}
