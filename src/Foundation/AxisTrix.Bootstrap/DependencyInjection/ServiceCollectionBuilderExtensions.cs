using Axis;
using AxisMediator;
using AxisMediator.Contracts.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.DependencyInjection;

public static class ServiceCollectionBuilderExtensions
{
    public static ServiceCollectionBuilder InitAxisTrixAdd(this IServiceCollection services)
    {
        var builder = new ServiceCollectionBuilder(services);
        return builder;
    }

    extension(ServiceCollectionBuilder builder)
    {
        public ServiceCollectionBuilder AddTelemetryBehavior()
        {
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<>), typeof(TelemetryBehavior<>));
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<,>), typeof(TelemetryBehavior<,>));
            return builder;
        }

        public IServiceCollection EndAxisTrixAdd(bool addDefaultBehaviors = true)
        {
            builder.Services
                .AddAxisLogger()
                .AddLoggingBehavior()
                .AddAxisMediator()
                .AddPerformanceBehavior()
                .AddOpenTelemetryAxis();

            if (addDefaultBehaviors)
                builder.AddTelemetryBehavior();

            return builder.Services;
        }
    }
}
