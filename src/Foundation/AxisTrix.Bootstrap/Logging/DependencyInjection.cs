using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Logging;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAxisLoggerFactore()
        {
            services.AddSingleton(AxisLoggerFactory.Create);
            services.AddLogging();
            return services;
        }
    }
}
