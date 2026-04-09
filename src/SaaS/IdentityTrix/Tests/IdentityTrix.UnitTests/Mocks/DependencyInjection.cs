using AxisTrix.Accessor;
using AxisTrix.DependencyInjection;
using IdentityTrix.Sdk.Application;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.UnitTests.Mocks;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void  AddMocks(IdentityTrixMocks identityTrixMocks)
        {
            services.AddSingleton(identityTrixMocks.UowProvider.Object);
            services.AddSingleton(identityTrixMocks.ExternalApiWriter.Object);
            services.AddSingleton(identityTrixMocks.ExternalApiReader.Object);
            services.AddSingleton(identityTrixMocks.Cache.Object);
        }

        public IdentityTrixMocks  AddSuccessfulMocks()
        {
            var mocks = IdentityTrixMocks.CreateSuccessfulMocks();
            services.AddMocks(mocks);
            return mocks;
        }

        public IServiceProvider GetServiceProvider()
        {
            var serviceProvider = services
                .InitAxisTrixAdd()
                .AddIdentityTrixSdkApplication()
                .EndAxisTrixAdd()
                .BuildServiceProvider();
            var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
            contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
            contextAccessor.PersonId = Guid.CreateVersion7().ToString();

            return serviceProvider;
        }
    }
}
