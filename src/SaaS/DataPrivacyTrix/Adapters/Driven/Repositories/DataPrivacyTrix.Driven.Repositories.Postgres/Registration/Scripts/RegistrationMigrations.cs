using Npgsql;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Registration.Scripts;

internal static class RegistrationMigrations
{
    private const string MigrationsTable = $"{RegistrationDbInit.Schema}.SCHEMA_MIGRATIONS";

    public static async Task InitializePostgresAsync(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using (var cmd = new NpgsqlCommand(
                         $"CREATE SCHEMA IF NOT EXISTS {RegistrationDbInit.Schema};" +
                         $"CREATE TABLE IF NOT EXISTS {MigrationsTable} " +
                         "(VERSION VARCHAR(50) PRIMARY KEY, APPLIED_AT TIMESTAMPTZ NOT NULL DEFAULT NOW())", conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        await using var transaction = await conn.BeginTransactionAsync();
        foreach (var (version, script) in RegistrationDbInit.Migrations)
        {
            await using var checkCmd = new NpgsqlCommand(
                $"SELECT 1 FROM {MigrationsTable} WHERE VERSION = @version", conn);
            checkCmd.Parameters.AddWithValue("version", version);
            if (await checkCmd.ExecuteScalarAsync() is not null)
                continue;

            await using (var migCmd = new NpgsqlCommand(script, conn, transaction))
                await migCmd.ExecuteNonQueryAsync();

            await using var recordCmd = new NpgsqlCommand($"INSERT INTO {MigrationsTable} (VERSION) VALUES (@version)", conn, transaction);
            recordCmd.Parameters.AddWithValue("version", version);
            await recordCmd.ExecuteNonQueryAsync();
        }
        await transaction.CommitAsync();
    }
}
