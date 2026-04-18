using Axis;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Driven.Repositories.Postgres.Tenants.Scripts;
using TenantTrix.IntegrationTests.Postgres;
using TenantTrix.Ports.Tenants;

namespace TenantTrix.IntegrationTests.Tenants;

public class TenantSeedDataTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
{
    [Fact]
    public async Task AdminTenantShouldExistAfterMigrationAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        using var scope = serviceProvider.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<ITenantsReaderPort>();

        var result = await reader.GetByIdAsync((TenantId)TenantsDbInit.SeedAdminTenantId);

        Assert.True(result.IsSuccess, $"Failed: {string.Join("; ", result.Errors.Select(e => e.Code))}");
        Assert.Equal(TenantsDbInit.SeedAdminTenantName, result.Value.TenantName);
    }

    [Fact]
    public void SeedAdminTenantIdShouldBeValidGuidV7()
    {
        TenantId id = TenantsDbInit.SeedAdminTenantId;
        Assert.Equal(TenantsDbInit.SeedAdminTenantId, id.ToString());
    }
}
