using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.SharedKernel.Cellphones;

public interface ICellphoneEntityProperties
{
    CellphoneId CellphoneId { get; }
    CountryId CountryId { get; }
    string CellphoneNumber { get; }
}
