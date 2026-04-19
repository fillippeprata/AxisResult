using DataPrivacyTrix.SharedKernel.Registration;
using Npgsql;
using AxisSecurityLevel = DataPrivacyTrix.SharedKernel.Registration.SecurityLevel;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Registration;

internal record AxisIdentityDbEntity(
    AxisIdentityId AxisIdentityId,
    bool IsIndividual,
    string Document,
    CountryId CountryId,
    string DisplayName,
    string DefaultLanguage,
    AxisSecurityLevel SecurityLevel) : IAxisIdentityEntityProperties
{
    internal static AxisIdentityDbEntity FromReader(NpgsqlDataReader reader)
        => new(
            reader.GetString(0),
            reader.GetBoolean(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            Enum.Parse<AxisSecurityLevel>(reader.GetString(6), ignoreCase: true));
}
