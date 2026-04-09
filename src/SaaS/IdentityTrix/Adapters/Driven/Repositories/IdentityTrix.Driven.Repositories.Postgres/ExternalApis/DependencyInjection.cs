using AxisTrix.DependencyInjection;
using IdentityTrix.Ports.ExternalApis;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Driven.Repositories.Postgres.ExternalApis;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddExternalApiRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ExternalApiRepository>();
        builder.Services.AddScoped<IExternalApiReaderPort>(sp => sp.GetRequiredService<ExternalApiRepository>());
        builder.Services.AddScoped<IExternalApiWritePort>(sp => sp.GetRequiredService<ExternalApiRepository>());
        return builder;
    }
}
