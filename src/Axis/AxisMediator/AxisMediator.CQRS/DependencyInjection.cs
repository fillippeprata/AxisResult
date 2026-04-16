using AxisMediator.Contracts;
using AxisMediator.Contracts.CQRS.Handlers;
using AxisMediator.Contracts.Pipelines;
using AxisMediator.CQRS.Handlers;
using AxisMediator.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace AxisMediator;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAxisMediator()
        {
            services.AddScoped<IAxisMediatorHandler, AxisMediatorHandler>();
            services.AddScoped<IAxisMediator, AxisMediator>();
            services.AddSingleton<IAxisMediatorAccessor, AxisMediatorAccessor>();
            services.AddSingleton<IAxisMediatorContextAccessor, AxisMediatorContextAccessor>();
            return services;
        }

        public IServiceCollection AddPerformanceBehavior()
        {
            services.AddTransient(typeof(IAxisPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            return services;
        }

    }
}
