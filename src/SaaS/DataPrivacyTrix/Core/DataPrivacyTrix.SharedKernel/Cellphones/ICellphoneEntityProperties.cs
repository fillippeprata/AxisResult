using AxisTrix.Types;

namespace DataPrivacyTrix.SharedKernel.Cellphones;

public interface ICellphoneEntityProperties
{
    CellphoneId CellphoneId { get; }
    CountryId CountryId { get; }
    string CellphoneNumber { get; }
}
