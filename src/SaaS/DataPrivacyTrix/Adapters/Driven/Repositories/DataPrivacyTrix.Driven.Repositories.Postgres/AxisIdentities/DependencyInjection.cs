using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Ports.AxisIdentities;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddAxisIdentitiesRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<AxisIdentitiesRepository>();
        builder.Services.AddScoped<IAxisIdentitiesReaderPort>(sp => sp.GetRequiredService<AxisIdentitiesRepository>());
        builder.Services.AddScoped<IAxisIdentitiesWritePort>(sp => sp.GetRequiredService<AxisIdentitiesRepository>());
        builder.Services.AddScoped<IAxisIdentityCellphonesWritePort>(sp => sp.GetRequiredService<AxisIdentitiesRepository>());
        builder.Services.AddScoped<IAxisIdentityEmailsWritePort>(sp => sp.GetRequiredService<AxisIdentitiesRepository>());
        return builder;
    }
}
