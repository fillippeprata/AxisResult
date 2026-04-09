using AxisTrix.DependencyInjection;
using IdentityTrix.Application.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Application.Authentication;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddAuthenticationModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ICachedExternalApiSecretResolver, CachedExternalApiSecretResolver>();
        return builder;
    }
}
