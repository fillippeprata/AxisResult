using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Application.AxisIdentities;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddRegistrationModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IAxisIdentityAggregateApplicationFactory, AxisIdentityAggregateApplicationFactory>();
        return builder;
    }
}
