using AxisTrix.DependencyInjection;
using AxisTrix.Repository.Postgres;
using IdentityTrix.Driven.Repositories.Postgres.ExternalApis;
using IdentityTrix.Driven.Repositories.Postgres.UnitOfWork;
using IdentityTrix.Ports;
using IdentityTrix.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Driven.Repositories.Postgres;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixPostgres(this ServiceCollectionBuilder builder, string connectionString)
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
