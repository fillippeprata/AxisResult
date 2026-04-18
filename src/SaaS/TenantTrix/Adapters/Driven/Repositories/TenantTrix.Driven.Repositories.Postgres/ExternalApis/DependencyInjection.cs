using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Ports.ExternalApis;

namespace TenantTrix.Driven.Repositories.Postgres.ExternalApis;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddExternalApiRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ExternalApisRepository>();
        builder.Services.AddScoped<IExternalApisReaderPort>(sp => sp.GetRequiredService<ExternalApisRepository>());
        builder.Services.AddScoped<IExternalApisWritePort>(sp => sp.GetRequiredService<ExternalApisRepository>());
        return builder;
    }
}
