using DataPrivacyTrix.SharedKernel.Emails;
using Npgsql;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Emails;

internal record EmailDbEntity(EmailId EmailId, string EmailAddress) : IEmailEntityProperties
{
    internal static EmailDbEntity FromReader(NpgsqlDataReader reader)
        => new(reader.GetString(0), reader.GetString(1));
}
