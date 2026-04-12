using AxisTrix.Types;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Domain.Cellphones.Root;

internal class CellphoneEntity(
    CellphoneId cellphoneId,
    CountryId countryId,
    string cellphoneNumber)
    : ICellphoneEntityProperties
{
    public CellphoneId CellphoneId { get; } = cellphoneId;
    public CountryId CountryId { get; } = countryId;
    public string CellphoneNumber { get; } = cellphoneNumber;

    internal CellphoneEntity(ICellphoneEntityProperties properties) : this(
        properties.CellphoneId,
        properties.CountryId,
        properties.CellphoneNumber
    ) { }
}
