using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Application.Cellphones;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddCellphonesModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ICellphoneAggregateApplicationFactory, CellphoneAggregateApplicationFactory>();
        return builder;
    }
}
