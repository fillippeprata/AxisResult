using AxisTrix.Accessor;
using AxisTrix.CQRS;
using AxisTrix.Logging;
using AxisTrix.Pipelines;
using AxisTrix.Pipelines.Behaviors;
using AxisTrix.Telemetry;
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

        public ServiceCollectionBuilder AddLoggingBehavior()
        {
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return builder;
        }

        public ServiceCollectionBuilder AddPerformanceBehavior()
        {
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            return builder;
        }

        public IServiceCollection EndAxisTrixAdd(bool addDefaultBehaviors = true)
        {
            builder
                .AddAxisLogger()
                .AddOpenTelemetryAxis()
                .AddMediatorHandler();

            if (addDefaultBehaviors)
                builder.AddTelemetryBehavior().AddLoggingBehavior().AddPerformanceBehavior();

            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddScoped<IAxisMediator, AxisMediator>();
            builder.Services.AddSingleton<IAxisMediatorAccessor, AxisMediatorAccessor>();
            builder.Services.AddSingleton<IAxisMediatorContextAccessor, AxisMediatorContextAccessor>();
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<>), typeof(ValidationBehavior<>));
            builder.Services.AddTransient(typeof(IAxisPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return builder.Services;
        }
    }
}
