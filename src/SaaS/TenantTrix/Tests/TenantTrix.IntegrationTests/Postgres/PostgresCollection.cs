namespace TenantTrix.IntegrationTests.Postgres;

/// <summary>
/// This informs that all tests in this collection will use the same database
/// </summary>
[CollectionDefinition("PostgresTenantTrixCollection")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>;
