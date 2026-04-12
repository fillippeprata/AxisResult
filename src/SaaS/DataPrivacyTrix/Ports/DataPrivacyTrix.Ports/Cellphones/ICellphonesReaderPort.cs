using AxisTrix.Results;
using AxisTrix.Types;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Ports.Cellphones;

public interface ICellphonesReaderPort
{
    Task<AxisResult<ICellphoneEntityProperties>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber);
    Task<AxisResult<ICellphoneEntityProperties>> GetByIdAsync(CellphoneId cellphoneId);
}
