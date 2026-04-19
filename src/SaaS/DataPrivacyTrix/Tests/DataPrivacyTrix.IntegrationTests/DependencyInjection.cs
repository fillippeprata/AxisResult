using AxisMediator.Contracts;
using AxisMemoryCache;
using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Driven.Repositories.Postgres;
using DataPrivacyTrix.Sdk.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.IntegrationTests;

public static class DependencyInjection
{
    public static IServiceProvider ServiceProviderWithPostgres(string connectionString)
    {
        var serviceProvider = new ServiceCollection()
            .AddAxisMemoryCache()
            .InitAxisTrixAdd()
            .AddDataPrivacyTrixPostgres(connectionString)
            .AddDataPrivacyTrixSdkApplication()
            .EndAxisTrixAdd()
            .BuildServiceProvider();
        var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
        contextAccessor.OriginId = $"DataPrivacyTrix-{Guid.NewGuid():N}";
        contextAccessor.PersonId = $"1|{Guid.CreateVersion7()}";
        return serviceProvider;
    }
}
