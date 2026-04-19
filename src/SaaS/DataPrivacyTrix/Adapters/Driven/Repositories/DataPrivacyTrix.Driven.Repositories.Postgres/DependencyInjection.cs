using AxisRepository.Postgres;
using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones;
using DataPrivacyTrix.Driven.Repositories.Postgres.Emails;
using DataPrivacyTrix.Driven.Repositories.Postgres.Registration;
using DataPrivacyTrix.Driven.Repositories.Postgres.UnitOfWork;
using DataPrivacyTrix.Ports;
using DataPrivacyTrix.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Driven.Repositories.Postgres;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddDataPrivacyTrixPostgres(this ServiceCollectionBuilder builder, string connectionString)
    {
        const string appKey = ApplicationConfig.AppKey;

        builder.Services.AddPostgresUnitOfWork(appKey, connectionString);
        builder.Services.AddScoped<IUnitOfWorkProvider, UnitOfWorkProvider>(
            sp => new(sp.GetRequiredKeyedService<IPostgresUnitOfWork>(appKey)));

        return builder
            .AddCellphonesRepository()
            .AddEmailsRepository()
            .AddAxisIdentitiesRepository();
    }
}
