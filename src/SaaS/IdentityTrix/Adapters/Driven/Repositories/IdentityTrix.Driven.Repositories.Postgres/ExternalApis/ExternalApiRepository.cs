using AxisTrix;
using AxisTrix.Logging;
using AxisTrix.Repository.Postgres;
using AxisTrix.Results;
using IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel;
using IdentityTrix.SharedKernel.ExternalApis;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Driven.Repositories.Postgres.ExternalApis;

internal class ExternalApiRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), IExternalApiReaderPort, IExternalApiWritePort
{
    private const string Select = $"SELECT {ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}";

    public Task<AxisResult> CreateAsync(IExternalApiEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {ExternalApisTable.Table} ({ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}) VALUES (@id, @name, @secret)",
            p =>
            {
                p.AddWithValue("id", Guid.Parse(properties.ExternalApiId.ToString()));
                p.AddWithValue("name", properties.ApiName);
                p.AddWithValue("secret", properties.HashedSecret);
            },
            duplicateKeyCode: "EXTERNAL_API_ALREADY_EXISTS");

    public Task<AxisResult<IExternalApiEntityProperties>> GetExternalApiByIdAsync(ExternalApiId id)
        => GetAsync<IExternalApiEntityProperties>(
            $"{Select} FROM {ExternalApisTable.Table} WHERE {ExternalApisTable.ExternalApiId} = @id",
            p => p.AddWithValue("id", id.ToString()),
            ExternalApiDbEntity.FromReader,
            "EXTERNAL_API_NOT_FOUND");

    public Task<AxisResult> UpdateSecretAsync(ExternalApiId id, string hashedSecret)
        => ExecuteAsync(
            $"UPDATE {ExternalApisTable.Table} SET {ExternalApisTable.Secret} = @secret WHERE {ExternalApisTable.ExternalApiId} = @id",
            p =>
            {
                p.AddWithValue("id", id.ToString());
                p.AddWithValue("secret", hashedSecret);
            });
}
