namespace DataPrivacyTrix.IntegrationTests.Postgres;

[Collection("PostgresDataPrivacyTrixCollection")]
public abstract class DatabaseTestBase(PostgresFixture fixture)
{
    protected readonly PostgresFixture Fixture = fixture;
}
