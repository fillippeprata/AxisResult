using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Application.ExternalApis;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddExternalApisModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IExternalApiAggregateApplicationFactory, ExternalApiAggregateApplicationFactory>();
        return builder;
    }
}
