using IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;
using IdentityTrix.IntegrationTests.Postgres;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.Contracts.ExternalApis.v1;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.IntegrationTests.ExternalApis;

public class ExternalApiSeedDataTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
{
    [Fact]
    public async Task AdminExternalApiShouldExistAfterMigrationAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);

        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

        var query = new GetExternalApiByIdQuery { ExternalApiId = ExternalApisDbInit.SeedAdminExternalApiId };
        var result = await mediator.GetByIdAsync(query);

        Assert.True(result.IsSuccess, $"Failed: {string.Join("; ", result.Errors.Select(e => e.Code))}");
        Assert.Equal(ExternalApisDbInit.SeedAdminApiName, result.Value.Name);
    }

    [Fact]
    public void SeedAdminExternalApiIdShouldBeValidGuidV7()
    {
        ExternalApiId id = ExternalApisDbInit.SeedAdminExternalApiId;
        Assert.Equal(ExternalApisDbInit.SeedAdminExternalApiId, id.ToString());
    }

    [Fact]
    public void SeedAdminSecretHashShouldMatchPlainSecret()
    {
        var hash = ExternalApiSecret.Hash(ExternalApisDbInit.SeedAdminPlainSecret);
        Assert.True(ExternalApiSecret.Verify(ExternalApisDbInit.SeedAdminPlainSecret, hash));
    }
}
