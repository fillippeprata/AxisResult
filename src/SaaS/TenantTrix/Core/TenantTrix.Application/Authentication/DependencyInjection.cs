using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Application.Authentication.Services;

namespace TenantTrix.Application.Authentication;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddAuthenticationModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ICachedExternalApiSecretResolver, CachedExternalApiSecretResolver>();
        return builder;
    }
}
