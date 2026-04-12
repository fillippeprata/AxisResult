using AxisTrix.Results;
using DataPrivacyTrix.SharedKernel.Cellphones;
using CountryId = AxisTrix.Types.Localization.CountryId;

namespace DataPrivacyTrix.Ports.Cellphones;

public interface ICellphonesReaderPort
{
    Task<AxisResult<ICellphoneEntityProperties>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber);
    Task<AxisResult<ICellphoneEntityProperties>> GetByIdAsync(CellphoneId cellphoneId);
}
