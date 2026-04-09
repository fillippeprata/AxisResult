using AxisTrix.Accessor;
using AxisTrix.Caching.Memory;
using AxisTrix.DependencyInjection;
using IdentityTrix.Driven.Repositories.Postgres;
using IdentityTrix.Sdk.Application;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.IntegrationTests;

public static class DependencyInjection
{
    public static IServiceProvider ServiceProviderWithPostgres(string connectionString)
    {
        var serviceProvider = new ServiceCollection()
            .InitAxisTrixAdd()
            .AddAxisMemoryCache()
            .AddIdentityTrixPostgres(connectionString)
            .AddIdentityTrixSdkApplication()
            .EndAxisTrixAdd()
            .BuildServiceProvider();
        var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
        contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
        contextAccessor.PersonId = Guid.CreateVersion7().ToString();
        return serviceProvider;
    }
}
