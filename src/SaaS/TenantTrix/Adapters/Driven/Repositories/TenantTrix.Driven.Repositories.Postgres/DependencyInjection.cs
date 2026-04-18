using AxisRepository.Postgres;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Driven.Repositories.Postgres.ExternalApis;
using TenantTrix.Driven.Repositories.Postgres.UnitOfWork;
using TenantTrix.Ports;
using TenantTrix.SharedKernel;

namespace TenantTrix.Driven.Repositories.Postgres;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddTenantTrixPostgres(this ServiceCollectionBuilder builder, string connectionString)
    {
        const string appKey = ApplicationConfig.AppKey;

        //DB Injection
        builder.Services.AddPostgresUnitOfWork(appKey, connectionString);
        builder.Services.AddScoped<IUnitOfWorkProvider, UnitOfWorkProvider>(
            sp => new(sp.GetRequiredKeyedService<IPostgresUnitOfWork>(appKey)));

        //Repositories
        return builder
            .AddExternalApiRepository();
    }
}
