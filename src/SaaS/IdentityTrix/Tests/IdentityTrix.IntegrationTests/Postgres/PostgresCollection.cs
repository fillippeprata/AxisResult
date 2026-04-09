namespace IdentityTrix.IntegrationTests.Postgres;

/// <summary>
/// This informs that all tests in this collection will use the same database
/// </summary>
[CollectionDefinition("PostgresIdentityTrixCollection")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>;
