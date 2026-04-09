using Npgsql;

namespace IdentityTrix.IntegrationTests.Postgres;

/// <summary>
/// Base class for all tests that require a Postgres database
/// </summary>
[Collection("PostgresIdentityTrixCollection")]
public abstract class DatabaseTestBase(PostgresFixture fixture)
{
    protected readonly PostgresFixture Fixture = fixture;
    protected NpgsqlConnection CreateConnection() => new (Fixture.ConnectionString);
}
