using IdentityTrix.IntegrationTests.Postgres;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1;
using IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;
using IndentityTrix.Contracts.ExternalApis.v1.GetById;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.IntegrationTests.ExternalApis;

public class ExternalApiRepositoryTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
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

            var cmd = new AddExternalApiCommand() { ApiName = "test-api-name" };
            var response = await Mediator(scope).AddAsync(cmd);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Name);
            Assert.NotEmpty(response.Value.Secret);
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
            var response = await Mediator(scope).GenerateNewSecretAsync(cmd);

            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Value.Secret);
        }
    }
}
