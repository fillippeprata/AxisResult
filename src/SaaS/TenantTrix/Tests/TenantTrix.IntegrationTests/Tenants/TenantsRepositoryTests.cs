using Axis;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Contracts.Tenants.v1;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Contracts.Tenants.v1.EditTenant;
using TenantTrix.IntegrationTests.Postgres;
using TenantTrix.Ports.Tenants;

namespace TenantTrix.IntegrationTests.Tenants;

public class TenantsRepositoryTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
{
    private static ITenantsMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<ITenantsMediator>();

    [Fact]
    public async Task TenantShouldInsertAndEditAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        string? tenantId;
        var initialName = $"tenant-{Guid.NewGuid():N}";
        var updatedName = $"renamed-{Guid.NewGuid():N}";

        using (var scope = serviceProvider.CreateScope())
        {
            var cmd = new AddTenantCommand { TenantName = initialName };
            var response = await Mediator(scope).AddAsync(cmd);

            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            Assert.NotEmpty(response.Value.TenantId);
            Assert.Equal(initialName, response.Value.TenantName);
            tenantId = response.Value.TenantId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var reader = scope.ServiceProvider.GetRequiredService<ITenantsReaderPort>();
            var result = await reader.GetByIdAsync((TenantId)tenantId);

            Assert.True(result.IsSuccess);
            Assert.Equal(initialName, result.Value.TenantName);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var cmd = new EditTenantCommand { TenantId = tenantId, TenantName = updatedName };
            var response = await Mediator(scope).EditAsync(cmd);

            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            Assert.Equal(updatedName, response.Value.TenantName);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var reader = scope.ServiceProvider.GetRequiredService<ITenantsReaderPort>();
            var result = await reader.GetByIdAsync((TenantId)tenantId);

            Assert.True(result.IsSuccess);
            Assert.Equal(updatedName, result.Value.TenantName);
        }
    }

    [Fact]
    public async Task AddTenantShouldFailWhenNameAlreadyExistsAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var name = $"duplicate-{Guid.NewGuid():N}";

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).AddAsync(new AddTenantCommand { TenantName = name });
            Assert.True(response.IsSuccess);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).AddAsync(new AddTenantCommand { TenantName = name });
            Assert.True(response.IsFailure);
            Assert.Contains(response.Errors, x => x.Code == "TENANT_NAME_ALREADY_EXISTS");
        }
    }

    [Fact]
    public async Task EditTenantShouldFailWhenNewNameBelongsToAnotherTenantAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var firstName = $"first-{Guid.NewGuid():N}";
        var secondName = $"second-{Guid.NewGuid():N}";
        string? secondTenantId;

        using (var scope = serviceProvider.CreateScope())
        {
            var r1 = await Mediator(scope).AddAsync(new AddTenantCommand { TenantName = firstName });
            var r2 = await Mediator(scope).AddAsync(new AddTenantCommand { TenantName = secondName });
            Assert.True(r1.IsSuccess);
            Assert.True(r2.IsSuccess);
            secondTenantId = r2.Value.TenantId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).EditAsync(new EditTenantCommand
            {
                TenantId = secondTenantId,
                TenantName = firstName
            });

            Assert.True(response.IsFailure);
            Assert.Contains(response.Errors, x => x.Code == "TENANT_NAME_ALREADY_EXISTS");
        }
    }
}
