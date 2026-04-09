using AxisTrix.DependencyInjection;
using AxisTrix.Telemetry.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Telemetry;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddOpenTelemetryAxis(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddSingleton<OpenTelemetryAdapter>();
        builder.Services.AddSingleton<IAxisTelemetry>(sp => sp.GetRequiredService<OpenTelemetryAdapter>());
        builder.Services.AddSingleton<IAxisMetrics>(sp => sp.GetRequiredService<OpenTelemetryAdapter>());
        return builder;
    }
}
