using Axis;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using Microsoft.Extensions.DependencyInjection;
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
}
