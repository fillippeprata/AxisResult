using DataPrivacyTrix.Driven.Repositories.Postgres;
using Testcontainers.PostgreSql;

namespace DataPrivacyTrix.IntegrationTests.Postgres;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container
        = new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("data_privacy_trix_test")
            .WithUsername("admin_test")
            .WithPassword("password_test")
            .WithCleanUp(true)
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await InitializeDatabaseAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    private Task InitializeDatabaseAsync()
        => DataPrivacyTrixMigrations.InitializePostgresAsync(ConnectionString);
}
