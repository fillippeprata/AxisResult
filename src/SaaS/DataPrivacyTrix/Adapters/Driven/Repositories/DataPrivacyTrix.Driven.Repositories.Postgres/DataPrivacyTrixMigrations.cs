using DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones.Scripts;
using DataPrivacyTrix.Driven.Repositories.Postgres.Emails.Scripts;
using DataPrivacyTrix.Driven.Repositories.Postgres.Registration.Scripts;

namespace DataPrivacyTrix.Driven.Repositories.Postgres;

public static class DataPrivacyTrixMigrations
{
    public static async Task InitializePostgresAsync(string connectionString)
    {
        await CellphonesMigrations.InitializePostgresAsync(connectionString);
        await EmailsMigrations.InitializePostgresAsync(connectionString);
        await RegistrationMigrations.InitializePostgresAsync(connectionString);
    }
}
