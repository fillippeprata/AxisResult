using Npgsql;

namespace TenantTrix.IntegrationTests.Postgres;

/// <summary>
/// Base class for all tests that require a Postgres database
/// </summary>
[Collection("PostgresTenantTrixCollection")]
public abstract class DatabaseTestBase(PostgresFixture fixture)
{
    protected readonly PostgresFixture Fixture = fixture;
    protected NpgsqlConnection CreateConnection() => new (Fixture.ConnectionString);
}
