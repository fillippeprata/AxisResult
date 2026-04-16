using Axis;
using DataPrivacyTrix.SharedKernel.Cellphones;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Ports.Cellphones;

public interface ICellphonesReaderPort
{
    Task<AxisResult<ICellphoneEntityProperties>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber);
    Task<AxisResult<ICellphoneEntityProperties>> GetByIdAsync(CellphoneId cellphoneId);
}
