using AxisMediator.Contracts;
using AxisMemoryCache;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Driven.Repositories.Postgres;
using TenantTrix.Sdk.Application;

namespace TenantTrix.IntegrationTests;

public static class DependencyInjection
{
    public static IServiceProvider ServiceProviderWithPostgres(string connectionString)
    {
        var serviceProvider = new ServiceCollection()
            .AddAxisMemoryCache()
            .InitAxisTrixAdd()
            .AddTenantTrixPostgres(connectionString)
            .AddTenantTrixSdkApplication()
            .EndAxisTrixAdd()
            .BuildServiceProvider();
        var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
        contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
        contextAccessor.PersonId = $"1|{Guid.CreateVersion7()}";
        return serviceProvider;
    }
}
