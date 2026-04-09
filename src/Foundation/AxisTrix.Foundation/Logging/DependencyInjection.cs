using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Logging;

internal static class DependencyInjection
{
    extension(ServiceCollectionBuilder builder)
    {
        public ServiceCollectionBuilder AddAxisLogger()
        {
            builder.Services.AddSingleton(AxisLoggerFactory.Create);
            builder.Services.AddLogging();
            builder.Services.AddScoped(typeof(IAxisLogger<>), typeof(AxisLogger<>));
            return builder;
        }
    }
}
