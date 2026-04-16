using AxisMediator.Contracts;
using AxisMemoryCache;
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
            .AddAxisMemoryCache()
            .InitAxisTrixAdd()
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
