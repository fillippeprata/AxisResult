using Axis;
using AxisMediator.Contracts;
using AxisRepository.Postgres;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Driven.Repositories.Postgres.Tenants.Scripts;
using TenantTrix.Ports.Tenants;
using TenantTrix.SharedKernel;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Driven.Repositories.Postgres.Tenants;

internal class TenantsRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), ITenantsReaderPort, ITenantsWritePort
{
    private const string Select = $"SELECT {TenantsTable.TenantId}, {TenantsTable.TenantName}";

    public Task<AxisResult> CreateAsync(ITenantEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {TenantsTable.Table} ({TenantsTable.TenantId}, {TenantsTable.TenantName}) VALUES (@id, @name)",
            p =>
            {
                p.AddWithValue("id", properties.TenantId.ToString());
                p.AddWithValue("name", properties.TenantName);
            },
            duplicateKeyCode: "TENANT_NAME_ALREADY_EXISTS");

    public Task<AxisResult<ITenantEntityProperties>> GetByIdAsync(TenantId id)
        => GetAsync<ITenantEntityProperties>(
            $"{Select} FROM {TenantsTable.Table} WHERE {TenantsTable.TenantId} = @id",
            p => p.AddWithValue("id", id.ToString()),
            TenantDbEntity.FromReader,
            "TENANT_NOT_FOUND");

    public Task<AxisResult<ITenantEntityProperties>> GetByNameAsync(string tenantName)
        => GetAsync<ITenantEntityProperties>(
            $"{Select} FROM {TenantsTable.Table} WHERE {TenantsTable.TenantName} = @name",
            p => p.AddWithValue("name", tenantName),
            TenantDbEntity.FromReader,
            "TENANT_NOT_FOUND");

    public Task<AxisResult> UpdateNameAsync(TenantId id, string tenantName)
        => ExecuteAsync(
            $"UPDATE {TenantsTable.Table} SET {TenantsTable.TenantName} = @name WHERE {TenantsTable.TenantId} = @id",
            p =>
            {
                p.AddWithValue("id", id.ToString());
                p.AddWithValue("name", tenantName);
            },
            duplicateKeyCode: "TENANT_NAME_ALREADY_EXISTS");
}
