using Axis;
using Axis.Localization;
using AxisValidator.Brazil;

namespace DataPrivacyTrix.Domain.Cellphones.Root;

internal partial class CellphoneEntity
{
    protected Task<AxisResult> IsValidAsync()
    {
        if (CountryId == CountryIds.Br)
            return Task.FromResult<AxisResult>(CountryId.FormatCellphone(CellphoneNumber));

        return Task.FromResult<AxisResult>(AxisError.BusinessRule("COUNTRY_ID_CELLPHONE_NOT_IMPLEMENTED"));
    }
}
