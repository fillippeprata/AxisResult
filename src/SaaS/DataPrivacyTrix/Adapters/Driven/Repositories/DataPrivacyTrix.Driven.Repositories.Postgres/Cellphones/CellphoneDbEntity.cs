using DataPrivacyTrix.SharedKernel.Cellphones;
using Npgsql;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones;

internal record CellphoneDbEntity(
    CellphoneId CellphoneId,
    CountryId CountryId,
    string CellphoneNumber) : ICellphoneEntityProperties
{
    internal static CellphoneDbEntity FromReader(NpgsqlDataReader reader)
        => new(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2));
}
