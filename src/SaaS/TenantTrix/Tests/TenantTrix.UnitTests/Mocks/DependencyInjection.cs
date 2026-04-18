using AxisMediator.Contracts;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Sdk.Application;

namespace TenantTrix.UnitTests.Mocks;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void  AddMocks(TenantTrixMocks identityTrixMocks)
        {
            services.AddSingleton(identityTrixMocks.UowProvider.Object);
            services.AddSingleton(identityTrixMocks.ExternalApiWriter.Object);
            services.AddSingleton(identityTrixMocks.ExternalApiReader.Object);
            services.AddSingleton(identityTrixMocks.Cache.Object);
        }

        public TenantTrixMocks  AddSuccessfulMocks()
        {
            var mocks = TenantTrixMocks.CreateSuccessfulMocks();
            services.AddMocks(mocks);
            return mocks;
        }

        public IServiceProvider GetServiceProvider()
        {
            var serviceProvider = services
                .InitAxisTrixAdd()
                .AddTenantTrixSdkApplication()
                .EndAxisTrixAdd()
                .BuildServiceProvider();
            var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
            contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
            contextAccessor.PersonId = Guid.CreateVersion7().ToString();

            return serviceProvider;
        }
    }
}
