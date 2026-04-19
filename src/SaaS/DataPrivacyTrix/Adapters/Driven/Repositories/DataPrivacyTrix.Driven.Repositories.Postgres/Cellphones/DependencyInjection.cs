using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Ports.Cellphones;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddCellphonesRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<CellphonesRepository>();
        builder.Services.AddScoped<ICellphonesReaderPort>(sp => sp.GetRequiredService<CellphonesRepository>());
        builder.Services.AddScoped<ICellphonesWritePort>(sp => sp.GetRequiredService<CellphonesRepository>());
        return builder;
    }
}
