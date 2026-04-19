using AxisMediator.Contracts;
using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Sdk.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.UnitTests.Mocks;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddMocks(DataPrivacyTrixMocks mocks)
        {
            services.AddSingleton(mocks.UowProvider.Object);
            services.AddSingleton(mocks.AxisIdentitiesReader.Object);
            services.AddSingleton(mocks.AxisIdentitiesWriter.Object);
            services.AddSingleton(mocks.AxisIdentityCellphonesWriter.Object);
            services.AddSingleton(mocks.AxisIdentityEmailsWriter.Object);
            services.AddSingleton(mocks.CellphonesReader.Object);
            services.AddSingleton(mocks.CellphonesWriter.Object);
            services.AddSingleton(mocks.EmailsReader.Object);
            services.AddSingleton(mocks.EmailsWriter.Object);
            services.AddSingleton(mocks.CellphonesMediator.Object);
            services.AddSingleton(mocks.EmailsMediator.Object);
        }

        public DataPrivacyTrixMocks AddSuccessfulMocks()
        {
            var mocks = DataPrivacyTrixMocks.CreateSuccessfulMocks();
            services.AddMocks(mocks);
            return mocks;
        }

        public IServiceProvider GetServiceProvider()
        {
            var serviceProvider = services
                .InitAxisTrixAdd()
                .AddDataPrivacyTrixSdkApplication()
                .EndAxisTrixAdd()
                .BuildServiceProvider();
            var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
            contextAccessor.OriginId = $"DataPrivacyTrix-{Guid.NewGuid():N}";
            contextAccessor.PersonId = $"1|{Guid.CreateVersion7()}";

            return serviceProvider;
        }
    }
}
