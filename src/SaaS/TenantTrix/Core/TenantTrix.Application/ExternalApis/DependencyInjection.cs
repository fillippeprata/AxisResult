using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Application.ExternalApis.Services;

namespace TenantTrix.Application.ExternalApis;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddExternalApisModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ICachedExternalApiSecretResolver, CachedExternalApiSecretResolver>();
        builder.Services.AddScoped<IExternalApiAggregateApplicationFactory, ExternalApiAggregateApplicationFactory>();
        return builder;
    }
}
