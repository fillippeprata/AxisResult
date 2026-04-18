using Axis;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using TenantTrix.IntegrationTests.Postgres;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.IntegrationTests.ExternalApis;

public class ExternalApisRepositoryTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
{
    private static IExternalApisMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

    [Fact]
    public async Task ExternalApiShouldInsertGetAndRenewSecretAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        ExternalApiId? apiId;

        using (var scope = serviceProvider.CreateScope())
        {
            var query = new GetExternalApiByIdQuery { ExternalApiId = "00000000-0000-7000-8000-000000000001"};
            var response = await Mediator(scope).GetByIdAsync(query);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Name);
        }

        using( var scope = serviceProvider.CreateScope())
        {

            var cmd = new AddExternalApiCommand() { ApiName = "test-api-name", TenantId = TenantId.New.ToString() };
            var response = await Mediator(scope).AddAsync(cmd);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Name);
            Assert.NotEmpty(response.Value.Secret);
            Assert.NotEmpty(response.Value.TenantId);
            apiId = response.Value.ExternalApiId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var query = new GetExternalApiByIdQuery { ExternalApiId = apiId};
            var response = await Mediator(scope).GetByIdAsync(query);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Name);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var cmd = new GenerateNewExternalApiSecretCommand() { ExternalApiId = apiId};
            var response = await Mediator(scope).GenerateNewExternalApiSecretAsync(cmd);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Secret);
        }
    }

    [Fact]
    public async Task EditExternalApiShouldUpdateNameAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var initialName = $"api-{Guid.NewGuid():N}";
        var updatedName = $"renamed-{Guid.NewGuid():N}";
        string? apiId;

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).AddAsync(new AddExternalApiCommand { ApiName = initialName, TenantId = TenantId.New.ToString() });
            Assert.True(response.IsSuccess);
            apiId = response.Value.ExternalApiId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).EditAsync(new EditExternalApiCommand { ExternalApiId = apiId, ApiName = updatedName });
            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            Assert.Equal(updatedName, response.Value.ApiName);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).GetByIdAsync(new GetExternalApiByIdQuery { ExternalApiId = apiId });
            Assert.True(response.IsSuccess);
            Assert.Equal(updatedName, response.Value.Name);
        }
    }

    [Fact]
    public async Task EditExternalApiShouldFailWhenNameBelongsToAnotherApiAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var firstName = $"first-{Guid.NewGuid():N}";
        var secondName = $"second-{Guid.NewGuid():N}";
        string? secondApiId;

        using (var scope = serviceProvider.CreateScope())
        {
            var r1 = await Mediator(scope).AddAsync(new AddExternalApiCommand { ApiName = firstName, TenantId = TenantId.New.ToString() });
            var r2 = await Mediator(scope).AddAsync(new AddExternalApiCommand { ApiName = secondName, TenantId = TenantId.New.ToString() });
            Assert.True(r1.IsSuccess);
            Assert.True(r2.IsSuccess);
            secondApiId = r2.Value.ExternalApiId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Mediator(scope).EditAsync(new EditExternalApiCommand { ExternalApiId = secondApiId, ApiName = firstName });
            Assert.True(response.IsFailure);
            Assert.Contains(response.Errors, x => x.Code == "EXTERNAL_API_NAME_ALREADY_EXISTS");
        }
    }
}
