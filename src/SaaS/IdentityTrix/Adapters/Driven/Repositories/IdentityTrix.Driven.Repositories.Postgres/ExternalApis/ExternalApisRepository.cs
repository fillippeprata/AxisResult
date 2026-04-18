using Axis;
using AxisMediator.Contracts;
using AxisRepository.Postgres;
using IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel;
using IdentityTrix.SharedKernel.ExternalApis;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Driven.Repositories.Postgres.ExternalApis;

internal class ExternalApisRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), IExternalApisReaderPort, IExternalApisWritePort
{
    private const string Select = $"SELECT {ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}, {ExternalApisTable.TenantId}";

    public Task<AxisResult> CreateAsync(IExternalApiEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {ExternalApisTable.Table} ({ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}, {ExternalApisTable.TenantId}) VALUES (@id, @name, @secret, @tenantId)",
            p =>
            {
                p.AddWithValue("id", Guid.Parse(properties.ExternalApiId.ToString()));
                p.AddWithValue("name", properties.ApiName);
                p.AddWithValue("secret", properties.HashedSecret);
                p.AddWithValue("tenantId", properties.TenantId.ToString());
            },
            duplicateKeyCode: "EXTERNAL_API_ALREADY_EXISTS");

    public Task<AxisResult<IExternalApiEntityProperties>> GetByIdAsync(ExternalApiId id)
        => GetAsync<IExternalApiEntityProperties>(
            $"{Select} FROM {ExternalApisTable.Table} WHERE {ExternalApisTable.ExternalApiId} = @id",
            p => p.AddWithValue("id", id.ToString()),
            ExternalApiDbEntity.FromReader,
            "EXTERNAL_API_NOT_FOUND");

    public Task<AxisResult<IExternalApiEntityProperties>> GetByNameAsync(string apiName)
        => GetAsync<IExternalApiEntityProperties>(
            $"{Select} FROM {ExternalApisTable.Table} WHERE {ExternalApisTable.Name} = @name",
            p => p.AddWithValue("name", apiName),
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
